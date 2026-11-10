import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_URL } from '../core/api';
import { Lesson } from '../Models/lesson';

export interface LessonBody {
  title: string;
  description: string;
  videoUrl: string;
  order: number;
}

@Injectable({ providedIn: 'root' })
export class LessonService {
  private http = inject(HttpClient);

  create(courseId: number, body: LessonBody) {
    return this.http.post<Lesson>(`${API_URL}/courses/${courseId}/lessons`, body);
  }

  update(courseId: number, lessonId: number, body: LessonBody) {
    return this.http.put<Lesson>(`${API_URL}/courses/${courseId}/lessons/${lessonId}`, body);
  }

  remove(courseId: number, lessonId: number) {
    return this.http.delete<void>(`${API_URL}/courses/${courseId}/lessons/${lessonId}`);
  }
}