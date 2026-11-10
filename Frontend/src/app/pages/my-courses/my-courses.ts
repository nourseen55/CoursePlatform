import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EnrollmentService } from '../../services/enrollment';
import { Enrollment } from '../../Models/enrollment';

@Component({
  selector: 'app-my-courses',
  imports: [RouterLink, DatePipe],
  templateUrl: './my-courses.html',
  styleUrl: './my-courses.css',
})
export class MyCourses implements OnInit {
  private enrollmentService = inject(EnrollmentService);

  enrollments = signal<Enrollment[]>([]);
  isLoading = signal(true);
  error = signal('');

  ngOnInit() {
    this.enrollmentService.getMy().subscribe({
      next: list => {
        this.enrollments.set(list);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('حصل خطأ في تحميل كورساتك.');
        this.isLoading.set(false);
      },
    });
  }
}