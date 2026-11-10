import { Component, OnInit, computed, inject, input, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { CourseService } from '../../services/course';
import { EnrollmentService } from '../../services/enrollment';
import { ProgressService } from '../../services/progress';
import { AuthService } from '../../core/auth';
import { CourseWithLessons } from '../../Models/course-with-lessons';
import { CourseProgress } from '../../Models/course-progress';

@Component({
  selector: 'app-course-details',
  imports: [RouterLink, DatePipe, DecimalPipe],
  templateUrl: './course-details.html',
  styleUrl: './course-details.css',
})
export class CourseDetails implements OnInit {
  private courseService = inject(CourseService);
  private enrollmentService = inject(EnrollmentService);
  private progressService = inject(ProgressService);
  auth = inject(AuthService);

  id = input.required<string>();
  courseId = computed(() => Number(this.id()));

  course = signal<CourseWithLessons | null>(null);
  isLoading = signal(true);
  error = signal('');

  isStudent = computed(() => this.auth.role() === 'Student');
  isEnrolled = signal(false);
  progress = signal<CourseProgress | null>(null);
  message = signal('');
  isBusy = signal(false);

  // المسودة والمؤرشف يظهروا لصاحب الكورس بس
  canView = computed(() => {
    const c = this.course();
    return !!c && (c.status === 'Published' || c.instructorId === this.auth.user()?.id);
  });

  sortedLessons = computed(() =>
    [...(this.course()?.lessons ?? [])].sort((a, b) => a.order - b.order)
  );

  ngOnInit() {
    this.loadCourse();
    if (this.isStudent()) {
      this.checkEnrollment();
    }
  }

  private loadCourse() {
    this.courseService.getById(this.courseId()).subscribe({
      next: c => {
        this.course.set(c);
        this.isLoading.set(false);
      },
      error: err => {
        this.error.set(err.status === 404
          ? 'الكورس ده مش موجود.'
          : 'حصل خطأ في تحميل الكورس.');
        this.isLoading.set(false);
      },
    });
  }

  private checkEnrollment() {
    this.enrollmentService.getMy().subscribe({
      next: list => {
        const enrolled = list.some(e => e.courseId === this.courseId());
        this.isEnrolled.set(enrolled);
        if (enrolled) this.loadProgress();
      },
    });
  }

  private loadProgress() {
    this.progressService.getProgress(this.courseId()).subscribe({
      next: p => this.progress.set(p),
    });
  }

  enroll() {
    this.isBusy.set(true);
    this.message.set('');

    this.enrollmentService.enroll(this.courseId()).subscribe({
      next: () => {
        this.isEnrolled.set(true);
        this.isBusy.set(false);
        this.loadProgress();
      },
      error: (err: HttpErrorResponse) => {
        this.message.set(this.errorText(err));
        this.isBusy.set(false);
      },
    });
  }

  completeLesson(lessonId: number) {
    this.isBusy.set(true);
    this.message.set('');

    this.progressService.completeLesson(this.courseId(), lessonId).subscribe({
      next: () => {
        this.isBusy.set(false);
        this.loadProgress();
      },
      error: (err: HttpErrorResponse) => {
        this.message.set(this.errorText(err));
        this.isBusy.set(false);
      },
    });
  }

  private errorText(err: HttpErrorResponse): string {
    if (err.status === 0) return 'مش قادر أوصل للسيرفر.';
    if (err.status === 409) return 'إنتي مشتركة في الكورس ده بالفعل.';
    if (err.status === 403) return 'ملكيش صلاحية للعملية دي.';
    return typeof err.error === 'string' ? err.error : 'حصل خطأ غير متوقع.';
  }
}