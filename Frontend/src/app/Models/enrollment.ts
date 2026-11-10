export interface Enrollment {
  id: number;
  courseId: number;
  courseTitle: string;
  status: string;        // 'Active' | 'Completed'
  enrolledAt: string;
}