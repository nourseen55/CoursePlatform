import { Component, OnInit, inject, signal } from '@angular/core';
import { Course } from '../../Models/course';
import { CourseCard } from '../../components/course-card/course-card';
import { CourseService } from '../../services/course';

@Component({
  selector: 'app-course-list',
  imports: [CourseCard],
  templateUrl: './course-list.html',
  styleUrl: './course-list.css',
})
export class CourseList implements OnInit {
  private courseService = inject(CourseService);

  courses = signal<Course[]>([]);
  isLoading = signal(true);
  error = signal('');

  ngOnInit() {
    this.courseService.getCourses().subscribe({
      next: page => {
        this.courses.set(page.items);
        this.isLoading.set(false);
      },
      error: err => {
        this.error.set(err.status === 0
          ? 'مش قادر أوصل للسيرفر. تأكدي إن الباك شغال وإن CORS مضبوط.'
          : 'حصل خطأ في تحميل الكورسات.');
        this.isLoading.set(false);
      },
    });
  }

  removeCourse(id: number) {
    this.courses.update(list => list.filter(c => c.id !== id));
  }
}