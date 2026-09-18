using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class Enrollment : BaseEntity
{
    public string StudentId { get; set; } = string.Empty;

    public int CourseId { get; set; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

    public DateTime EnrolledAt { get; set; }

    // Navigation Properties

    //public ApplicationUser Student { get; set; } = null!;

    public Course Course { get; set; } = null!;

    public ICollection<LessonProgress> LessonProgresses { get; set; }
        = new List<LessonProgress>();
}
