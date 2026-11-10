import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_URL } from '../core/api';
import { CourseProgress } from '../Models/course-progress';

@Injectable({ providedIn: 'root' })
export class ProgressService {
  private http = inject(HttpClient);

  getProgress(courseId: number) {
    return this.http.get<CourseProgress>(`${API_URL}/Progress/${courseId}/progress`);
  }

  completeLesson(courseId: number, lessonId: number) {
    return this.http.post<void>(
      `${API_URL}/Progress/${courseId}/lessons/${lessonId}/complete`, {});
  }
}