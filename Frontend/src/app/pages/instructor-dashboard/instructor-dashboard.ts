import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { CourseService } from '../../services/course';
import { errorText } from '../../core/http-error';
import { StatusLabelPipe } from '../../shared/status-label.pipe';
import { InstructorCourse } from '../../Models/instructor-course';
import { ConfirmService } from '../../core/confirm';

@Component({
  selector: 'app-instructor-dashboard',
  imports: [ReactiveFormsModule, RouterLink, DatePipe, StatusLabelPipe],
  templateUrl: './instructor-dashboard.html',
  styleUrl: './instructor-dashboard.css',
})
export class InstructorDashboard implements OnInit {
  private courseService = inject(CourseService);
  private fb = inject(FormBuilder);
  private confirmService = inject(ConfirmService);

  courses = signal<InstructorCourse[]>([]);
  isLoading = signal(true);
  isCreating = signal(false);
  busyId = signal<number | null>(null);   // رقم الكورس اللي بيتنفّذ عليه عملية
  errorMessage = signal('');

  stats = computed(() => {
    const list = this.courses();
    return {
      total: list.length,
      draft: list.filter(c => c.status === 'Draft').length,
      published: list.filter(c => c.status === 'Published').length,
    };
  });

  form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.maxLength(2000)]],
  });

  ngOnInit() {
    this.load();
  }

  private load() {
    this.courseService.getMine().subscribe({
      next: list => {
        this.courses.set(list);
        this.isLoading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.errorMessage.set(errorText(err));
        this.isLoading.set(false);
      },
    });
  }

  create() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isCreating.set(true);
    this.errorMessage.set('');

    this.courseService.create(this.form.getRawValue()).subscribe({
      next: () => {
        this.form.reset();
        this.isCreating.set(false);
        this.load();
      },
      error: (err: HttpErrorResponse) => {
        this.errorMessage.set(errorText(err));
        this.isCreating.set(false);
      },
    });
  }

  publish(course: InstructorCourse) {
    this.run(course.id, this.courseService.publish(course.id));
  }

  async archive(course: InstructorCourse) {
  const ok = await this.confirmService.ask({
    title: 'أرشفة الكورس',
    message: `هتأرشفي "${course.title}"؟ الكورس هيتقفل ومش هينفع ترجّعيه تاني.`,
    confirmText: 'أرشفة',
  });
  if (!ok) return;

  this.run(course.id, this.courseService.archive(course.id));
}

async remove(course: InstructorCourse) {
  const ok = await this.confirmService.ask({
    title: 'حذف الكورس',
    message: `هتحذفي "${course.title}" نهائياً؟ مفيش رجوع بعد كده.`,
    confirmText: 'حذف نهائي',
    danger: true,
  });
  if (!ok) return;

  this.run(course.id, this.courseService.remove(course.id));
}

  private run(id: number, request$: Observable<unknown>) {
    this.busyId.set(id);
    this.errorMessage.set('');

    request$.subscribe({
      next: () => {
        this.busyId.set(null);
        this.load();
      },
      error: (err: HttpErrorResponse) => {
        this.errorMessage.set(errorText(err));
        this.busyId.set(null);
      },
    });
  }
}