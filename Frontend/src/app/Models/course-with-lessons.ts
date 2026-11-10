import { Course } from './course';
import { Lesson } from './lesson';

export interface CourseWithLessons extends Course {
  lessons: Lesson[];
}
