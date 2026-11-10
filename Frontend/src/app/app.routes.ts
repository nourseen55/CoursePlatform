import { Routes } from '@angular/router';
import { inject } from '@angular/core';
import { CourseList } from './pages/course-list/course-list';
import { CourseDetails } from './pages/course-details/course-details';
import { NotFound } from './pages/not-found/not-found';
import { Login } from './pages/login/login';
import { MyCourses } from './pages/my-courses/my-courses';
import { roleGuard } from './core/guards';
import { InstructorDashboard } from './pages/instructor-dashboard/instructor-dashboard';
import { AuthService } from './core/auth';
import { ManageCourse } from './pages/manage-course/manage-course';
import { AdminUsers } from './pages/admin-users/admin-users';



export const routes: Routes = [
{ path: '', pathMatch: 'full', redirectTo: () => inject(AuthService).homeUrl() },  { path: 'instructor', component: InstructorDashboard, canActivate: [roleGuard('Instructor')] },
{ path: 'instructor/courses/:id', component: ManageCourse, canActivate: [roleGuard('Instructor')] }, 
{ path: 'my-courses', component: MyCourses, canActivate: [roleGuard('Student')] },
  { path: 'courses', component: CourseList },
  { path: 'admin', component: AdminUsers, canActivate: [roleGuard('Admin')] },
  { path: 'courses/:id', component: CourseDetails },
  { path: 'login', component: Login },
  { path: '**', component: NotFound },
];