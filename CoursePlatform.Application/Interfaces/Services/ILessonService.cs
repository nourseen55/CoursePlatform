using CoursePlatform.Application.DTOs.Lessons;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Interfaces.Services
{
    public interface ILessonService
    {
        Task<(LessonActionResult result, LessonResponse? data)> CreateAsync(
            int courseId,
            CreateLessonRequest request,
            string instructorId);
        Task<(LessonActionResult result, LessonResponse? data)> GetByIdAsync(
    int lessonId);
        Task<LessonActionResult> DeleteAsync(
    int lessonId,
    string instructorId);
    }
}
