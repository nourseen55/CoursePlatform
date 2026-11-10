
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_URL } from '../core/api';
import { User } from '../Models/user';

@Injectable({ providedIn: 'root' })
export class UserService {
  private http = inject(HttpClient);

  getAll() {
    return this.http.get<User[]>(`${API_URL}/users`);
  }

  create(body: {
    fullName: string;
    email: string;
    password: string;
    role: string;
  }) {
    return this.http.post<User>(`${API_URL}/users/create`, body);
  }

  update(
    id: string,
    body: {
      fullName: string;
      email: string;
      isActive: boolean;
    }
  ) {
    return this.http.put<void>(`${API_URL}/users/${id}`, body);
  }
}
