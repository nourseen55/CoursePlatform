import { Component, ElementRef, OnInit, computed, inject, input, signal, viewChild } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CourseService } from '../../services/course';
import { LessonService } from '../../services/lesson';
import { AuthService } from '../../core/auth';
import { ConfirmService } from '../../core/confirm';
import { errorText } from '../../core/http-error';
import { StatusLabelPipe } from '../../shared/status-label.pipe';
import { CourseWithLessons } from '../../Models/course-with-lessons';
import { Lesson } from '../../Models/lesson';

@Component({
  selector: 'app-manage-course',
  imports: [ReactiveFormsModule, RouterLink, StatusLabelPipe],
  templateUrl: './manage-course.html',
  styleUrl: './manage-course.css',
})
export class ManageCourse implements OnInit {
  private courseService = inject(CourseService);
  private lessonService = inject(LessonService);
  private auth = inject(AuthService);
  private confirmService = inject(ConfirmService);
  private fb = inject(FormBuilder);

  id = input.required<string>();
  courseId = computed(() => Number(this.id()));

  course = signal<CourseWithLessons | null>(null);
  isLoading = signal(true);
  loadError = signal('');
  errorMessage = signal('');
  successMessage = signal('');

  isSavingCourse = signal(false);
  isSavingLesson = signal(false);
  busyLessonId = signal<number | null>(null);
  editingLessonId = signal<number | null>(null);

  isOwner = computed(() => this.course()?.instructorId === this.auth.user()?.id);
  isArchived = computed(() => this.course()?.status === 'Archived');
  canEdit = computed(() => this.isOwner() && !this.isArchived());

  sortedLessons = computed(() =>
    [...(this.course()?.lessons ?? [])].sort((a, b) => a.order - b.order)
  );

  lessonCard = viewChild<ElementRef<HTMLElement>>('lessonCard');

  courseForm = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.maxLength(2000)]],
  });

  lessonForm = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.maxLength(2000)]],
    videoUrl: ['', [Validators.required, Validators.maxLength(500), Validators.pattern(/^https?:\/\/.+/)]],
    order: [1, [Validators.required, Validators.min(1)]],
  });

  ngOnInit() {
    this.load(true, () => this.resetLessonForm());
  }

  private load(patchCourseForm = false, after?: () => void) {
    this.courseService.getById(this.courseId()).subscribe({
      next: c => {
        this.course.set(c);
        if (patchCourseForm) {
          this.courseForm.setValue({ title: c.title, description: c.description });
        }
        this.applyPermissions();
        this.isLoading.set(false);
        after?.();
      },
      error: (err: HttpErrorResponse) => {
        const message = err.status === 404 ? 'الكورس ده مش موجود.' : errorText(err);
        if (this.course()) this.errorMessage.set(message);
        else this.loadError.set(message);
        this.isLoading.set(false);
      },
    });
  }

  private applyPermissions() {
    if (this.canEdit()) {
      this.courseForm.enable();
      this.lessonForm.enable();
    } else {
      this.courseForm.disable();
      this.lessonForm.disable();
    }
  }

  private clearMessages() {
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  // ===== بيانات الكورس =====
  saveCourse() {
    if (this.courseForm.invalid) {
      this.courseForm.markAllAsTouched();
      return;
    }

    this.clearMessages();
    this.isSavingCourse.set(true);

    this.courseService.update(this.courseId(), this.courseForm.getRawValue()).subscribe({
      next: updated => {
        this.course.update(c =>
          c ? { ...c, title: updated.title, description: updated.description } : c
        );
        this.courseForm.markAsPristine();
        this.successMessage.set('تم حفظ بيانات الكورس.');
        this.isSavingCourse.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.errorMessage.set(errorText(err));
        this.isSavingCourse.set(false);
      },
    });
  }

  // ===== الدروس =====
  private nextOrder(): number {
    const orders = (this.course()?.lessons ?? []).map(l => l.order);
    return orders.length ? Math.max(...orders) + 1 : 1;
  }

  private resetLessonForm() {
    this.lessonForm.reset({
      title: '',
      description: '',
      videoUrl: '',
      order: this.nextOrder(),
    });
  }

  startEdit(lesson: Lesson) {
    this.clearMessages();
    this.editingLessonId.set(lesson.id);
    this.lessonForm.setValue({
      title: lesson.title,
      description: lesson.description,
      videoUrl: lesson.videoUrl,
      order: lesson.order,
    });
    this.lessonCard()?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  cancelEdit() {
    this.editingLessonId.set(null);
    this.resetLessonForm();
  }

  saveLesson() {
    if (this.lessonForm.invalid) {
      this.lessonForm.markAllAsTouched();
      return;
    }

    const body = this.lessonForm.getRawValue();
    const editingId = this.editingLessonId();

    const request$ = editingId === null
      ? this.lessonService.create(this.courseId(), body)
      : this.lessonService.update(this.courseId(), editingId, body);

    this.clearMessages();
    this.isSavingLesson.set(true);

    request$.subscribe({
      next: () => {
        this.successMessage.set(editingId === null ? 'تمت إضافة الدرس.' : 'تم تحديث الدرس.');
        this.editingLessonId.set(null);
        this.isSavingLesson.set(false);
        this.load(false, () => this.resetLessonForm());
      },
      error: (err: HttpErrorResponse) => {
        this.errorMessage.set(errorText(err));
        this.isSavingLesson.set(false);
      },
    });
  }

  async removeLesson(lesson: Lesson) {
    const ok = await this.confirmService.ask({
      title: 'حذف الدرس',
      message: `هتحذفي "${lesson.title}"؟ مفيش رجوع بعد كده.`,
      confirmText: 'حذف',
      danger: true,
    });
    if (!ok) return;

    this.clearMessages();
    this.busyLessonId.set(lesson.id);

    this.lessonService.remove(this.courseId(), lesson.id).subscribe({
      next: () => {
        this.busyLessonId.set(null);
        if (this.editingLessonId() === lesson.id) this.editingLessonId.set(null);
        this.successMessage.set('تم حذف الدرس.');
        this.load(false, () => {
          if (this.editingLessonId() === null) this.resetLessonForm();
        });
      },
      error: (err: HttpErrorResponse) => {
        this.errorMessage.set(errorText(err));
        this.busyLessonId.set(null);
      },
    });
  }
}