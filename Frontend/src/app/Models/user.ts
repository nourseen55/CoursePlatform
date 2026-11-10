
export interface User {
  id: string;
  fullName: string;
  email: string;
  role: string;        // 'Admin' | 'Instructor' | 'Student'
  isActive: boolean;
}

export interface LoginResponse {
  accessToken: string;
  expiresAt: string;   // تاريخ انتهاء التوكن
  user: User;
}