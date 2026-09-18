using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string InstructorId { get; set; } = string.Empty;

    public CourseStatus Status { get; set; } = CourseStatus.Draft;

    // Navigation Properties

    //public ApplicationUser Instructor { get; set; } = null!;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
