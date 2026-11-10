import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { API_URL } from '../core/api';
import { Course } from '../Models/course';
import { CourseWithLessons } from '../Models/course-with-lessons';
import { InstructorCourse } from '../Models/instructor-course';
import { PaginatedResponse } from '../Models/paginated-response';

@Injectable({ providedIn: 'root' })
export class CourseService {
  private http = inject(HttpClient);

  // ===== للكل =====
  getCourses(pageNumber = 1, pageSize = 10) {
    const params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.http.get<PaginatedResponse<Course>>(`${API_URL}/courses`, { params });
  }

  getById(id: number) {
    return this.http.get<CourseWithLessons>(`${API_URL}/courses/${id}`);
  }

  // ===== للمدرّس =====
  getMine() {
    return this.http.get<InstructorCourse[]>(`${API_URL}/courses/mine`);
  }

  create(body: { title: string; description: string }) {
    return this.http.post<Course>(`${API_URL}/courses`, body);
  }

  update(id: number, body: { title: string; description: string }) {
    return this.http.put<Course>(`${API_URL}/courses/${id}`, body);
  }

  publish(id: number) {
    return this.http.post<void>(`${API_URL}/courses/${id}/publish`, {});
  }

  archive(id: number) {
    return this.http.post<void>(`${API_URL}/courses/${id}/archive`, {});
  }

  remove(id: number) {
    return this.http.delete<void>(`${API_URL}/courses/${id}`);
  }
}