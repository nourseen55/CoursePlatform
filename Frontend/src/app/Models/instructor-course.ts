import { Course } from './course';

export interface InstructorCourse extends Course {
  lessonsCount: number;
}