namespace CoursePlatform.Domain.Entities;

public class LessonProgress : BaseEntity
{
    public int EnrollmentId { get; set; }

    public int LessonId { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }

    // Navigation Properties

    public Enrollment Enrollment { get; set; } = null!;

    public Lesson Lesson { get; set; } = null!;
}