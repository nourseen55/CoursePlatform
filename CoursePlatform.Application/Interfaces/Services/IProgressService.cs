using CoursePlatform.Application.DTOs.Lessons;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Interfaces.Services
{
    public interface IProgressService
    {
        Task<ProgressActionResult> CompleteLessonAsync(
            int courseId,
            int lessonId,
            string studentId);

        Task<(ProgressActionResult result,
              CourseProgressResponse? data)>
            GetCourseProgressAsync(
                int courseId,
                string studentId);
    }
}
