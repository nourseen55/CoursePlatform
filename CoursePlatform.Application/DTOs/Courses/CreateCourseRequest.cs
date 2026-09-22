namespace CoursePlatform.Application.DTOs.Courses;

public class CreateCourseRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
public class CourseResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string InstructorId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}