import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  isSubmitting = signal(false);
  error = signal('');

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();   // يظهر كل رسائل الأخطاء
      return;
    }

    this.isSubmitting.set(true);
    this.error.set('');

    this.auth.login(this.form.getRawValue()).subscribe({
      next: () => this.router.navigateByUrl(this.auth.homeUrl()),
      error: err => {
        this.error.set(
          err.status === 401 ? 'الإيميل أو الباسورد غلط.'
          : err.status === 0 ? 'مش قادر أوصل للسيرفر.'
          : 'حصل خطأ غير متوقع.',
        );
        this.isSubmitting.set(false);
      },
    });
  }
}