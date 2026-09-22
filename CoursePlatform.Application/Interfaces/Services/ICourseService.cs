using CoursePlatform.Application.DTOs.Common;
using CoursePlatform.Application.DTOs.Courses;
using CoursePlatform.Domain.Entities;

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
    }
}
