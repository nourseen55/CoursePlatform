namespace CoursePlatform.Domain.Entities;

public class Lesson : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string VideoUrl { get; set; } = string.Empty;

    public int Order { get; set; }

    public int CourseId { get; set; }

    // Navigation Property

    public Course Course { get; set; } = null!;

    public ICollection<LessonProgress> Progresses { get; set; }
        = new List<LessonProgress>();
}