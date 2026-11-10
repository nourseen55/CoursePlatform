namespace CoursePlatform.Application.DTOs.Lessons;

public class CreateLessonRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string VideoUrl { get; set; } = string.Empty;

    public int Order { get; set; }
}

public class LessonResponse
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string VideoUrl { get; set; } = string.Empty;

    public int Order { get; set; }

    public DateTime CreatedAt { get; set; }
}
public class UpdateLessonRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string VideoUrl { get; set; } = string.Empty;

    public int Order { get; set; }
}