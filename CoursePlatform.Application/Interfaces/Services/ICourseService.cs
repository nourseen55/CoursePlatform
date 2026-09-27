using CoursePlatform.Application.DTOs.Common;
using CoursePlatform.Application.DTOs.Courses;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Interfaces.Services
{
    public interface ICourseService
    {
        Task<CourseResponse> CreateAsync(
            CreateCourseRequest request,
            string instructorId);
        Task<PaginatedResponse<CourseWithLessonsResponse>> GetPagedAsync(
          int pageNumber,
          int pageSize);
        Task<(CourseActionResult result, CourseWithLessonsResponse? data)>
    GetByIdAsync(int courseId);
        Task<CourseActionResult> PublishAsync(
    int courseId,
    string instructorId);
        Task<CourseActionResult> ArchiveAsync(
    int courseId,
    string instructorId);
        Task<(CourseAccessResult result, CourseWithLessonsResponse? data)>
    GetForStudentAsync(
        int courseId,
        string studentId);
    }
}
