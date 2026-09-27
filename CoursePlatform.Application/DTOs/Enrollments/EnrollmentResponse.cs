namespace CoursePlatform.Application.DTOs.Enrollments
{
    public class EnrollmentResponse
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; }
    }
}
