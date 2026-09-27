namespace CoursePlatform.Application.DTOs.Lessons
{
    public class CourseProgressResponse
    {
        public int CourseId { get; set; }
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public decimal ProgressPercentage { get; set; }
    }
}
