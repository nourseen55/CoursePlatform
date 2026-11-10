import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_URL } from '../core/api';
import { Enrollment } from '../Models/enrollment';

@Injectable({ providedIn: 'root' })
export class EnrollmentService {
  private http = inject(HttpClient);

  enroll(courseId: number) {
    return this.http.post<Enrollment>(`${API_URL}/courses/${courseId}/enroll`, {});
  }

  getMy() {
    return this.http.get<Enrollment[]>(`${API_URL}/courses/my`);
  }
}