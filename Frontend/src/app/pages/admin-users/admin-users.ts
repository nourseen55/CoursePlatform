
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable } from 'rxjs';

import { UserService } from '../../services/user';
import { AuthService } from '../../core/auth';
import { ConfirmService } from '../../core/confirm';
import { errorText } from '../../core/http-error';
import { User } from '../../Models/user';

type Mode = 'create' | 'edit';

@Component({
  selector: 'app-admin-users',
  imports: [ReactiveFormsModule],
  templateUrl: './admin-users.html',
  styleUrl: './admin-users.css',
})
export class AdminUsers implements OnInit {
  private userService = inject(UserService);
  private confirmService = inject(ConfirmService);
  private fb = inject(FormBuilder);

  auth = inject(AuthService);

  users = signal<User[]>([]);
  isLoading = signal(true);
  isSaving = signal(false);
  busyUserId = signal<string | null>(null);

  errorMessage = signal('');
  successMessage = signal('');

  mode = signal<Mode>('create');
  selected = signal<User | null>(null);

  search = signal('');
  roleFilter = signal('all');

  roles = ['Student', 'Instructor', 'Admin'];

  roleLabels: Record<string, string> = {
    Admin: 'أدمن',
    Instructor: 'مدرّس',
    Student: 'طالب',
  };

  stats = computed(() => {
    const list = this.users();

    return {
      total: list.length,
      instructors: list.filter(u => u.role === 'Instructor').length,
      students: list.filter(u => u.role === 'Student').length,
      inactive: list.filter(u => !u.isActive).length,
    };
  });

  filtered = computed(() => {
    const q = this.search().trim().toLowerCase();
    const role = this.roleFilter();

    return this.users().filter(u =>
      (role === 'all' || u.role === role) &&
      (
        !q ||
        u.fullName.toLowerCase().includes(q) ||
        u.email.toLowerCase().includes(q)
      )
    );
  });

  createForm = this.fb.nonNullable.group({
    fullName: ['', [
      Validators.required,
      Validators.maxLength(150),
    ]],

    email: ['', [
      Validators.required,
      Validators.email,
    ]],

    password: ['', [
      Validators.required,
      Validators.pattern(
        /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$/
      ),
    ]],

    role: ['Student', Validators.required],
  });

  editForm = this.fb.nonNullable.group({
    fullName: ['', [
      Validators.required,
      Validators.maxLength(150),
    ]],

    email: ['', [
      Validators.required,
      Validators.email,
    ]],

    isActive: [true],
  });

  ngOnInit(): void {
    this.load();
  }

  private load(): void {
    this.isLoading.set(true);

    this.userService.getAll().subscribe({
      next: list => {
        this.users.set(list);
        this.isLoading.set(false);
      },

      error: (err: HttpErrorResponse) => {
        this.errorMessage.set(errorText(err));
        this.isLoading.set(false);
      },
    });
  }

  private clearMessages(): void {
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  isSelf(user: User): boolean {
    return user.id === this.auth.user()?.id;
  }

  backToCreate(): void {
    this.mode.set('create');
    this.selected.set(null);
    this.editForm.controls.isActive.enable();
  }

  startEdit(user: User): void {
    this.clearMessages();

    this.mode.set('edit');
    this.selected.set(user);

    this.editForm.setValue({
      fullName: user.fullName,
      email: user.email,
      isActive: user.isActive,
    });

    if (this.isSelf(user)) {
      this.editForm.controls.isActive.disable();
    } else {
      this.editForm.controls.isActive.enable();
    }
  }

  create(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    this.clearMessages();
    this.isSaving.set(true);

    this.userService.create(this.createForm.getRawValue()).subscribe({
      next: () => {
        this.successMessage.set('تم إنشاء المستخدم بنجاح.');

        this.createForm.reset({
          fullName: '',
          email: '',
          password: '',
          role: 'Student',
        });

        this.isSaving.set(false);
        this.load();
      },

      error: (err: HttpErrorResponse) => {
        this.errorMessage.set(errorText(err));
        this.isSaving.set(false);
      },
    });
  }

  saveEdit(): void {
    const user = this.selected();

    if (!user) return;

    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    this.clearMessages();
    this.isSaving.set(true);

    this.userService.update(user.id, this.editForm.getRawValue()).subscribe({
      next: () => {
        this.successMessage.set('تم تحديث بيانات المستخدم.');

        this.isSaving.set(false);
        this.backToCreate();
        this.load();
      },

      error: (err: HttpErrorResponse) => {
        this.errorMessage.set(errorText(err));
        this.isSaving.set(false);
      },
    });
  }

  async toggleActive(user: User): Promise<void> {
    const deactivating = user.isActive;

    const ok = await this.confirmService.ask({
      title: deactivating ? 'تعطيل الحساب' : 'تفعيل الحساب',

      message: deactivating
        ? `"${user.fullName}" مش هيقدر يسجّل دخول بعد كده. تكمّلي؟`
        : `هتفعّلي حساب "${user.fullName}"؟`,

      confirmText: deactivating ? 'تعطيل' : 'تفعيل',
      danger: deactivating,
    });

    if (!ok) return;

    this.run(
      user.id,
      this.userService.update(user.id, {
        fullName: user.fullName,
        email: user.email,
        isActive: !user.isActive,
      }),
      deactivating ? 'تم تعطيل الحساب.' : 'تم تفعيل الحساب.',
    );
  }

  private run(
    id: string,
    request$: Observable<unknown>,
    success: string
  ): void {
    this.clearMessages();
    this.busyUserId.set(id);

    request$.subscribe({
      next: () => {
        this.busyUserId.set(null);
        this.successMessage.set(success);
        this.load();
      },

      error: (err: HttpErrorResponse) => {
        this.errorMessage.set(errorText(err));
        this.busyUserId.set(null);
      },
    });
  }
}
