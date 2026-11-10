import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { API_URL } from './api';
import { User, LoginResponse } from '../Models/user';

const TOKEN_KEY = 'token';
const USER_KEY = 'user';
const EXPIRES_KEY = 'expiresAt';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  user = signal<User | null>(this.loadUser());
  isLoggedIn = computed(() => this.user() !== null);
  role = computed(() => this.user()?.role ?? null);
  homeUrl = computed(() => {
  switch (this.role()) {
    case 'Admin':
      return '/admin';
    case 'Instructor':
      return '/instructor';
    default:
      return '/courses';
  }
});
  get token(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  login(body: { email: string; password: string }) {
    return this.http.post<LoginResponse>(`${API_URL}/auth/login`, body).pipe(
      tap(res => {
        localStorage.setItem(TOKEN_KEY, res.accessToken);
        localStorage.setItem(EXPIRES_KEY, res.expiresAt);
        localStorage.setItem(USER_KEY, JSON.stringify(res.user));
        this.user.set(res.user);
      }),
    );
  }

  logout() {
    this.clearStorage();
    this.user.set(null);
    this.router.navigate(['/login']);
  }

  private loadUser(): User | null {
    try {
      const expires = localStorage.getItem(EXPIRES_KEY);
      if (!expires || new Date(expires) <= new Date()) {
        this.clearStorage();
        return null;
      }
      return JSON.parse(localStorage.getItem(USER_KEY) ?? 'null');
    } catch {
      return null;
    }
  }

  private clearStorage() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(EXPIRES_KEY);
    localStorage.removeItem(USER_KEY);
  }
}