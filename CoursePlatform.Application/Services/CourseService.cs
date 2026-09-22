using CoursePlatform.Application.DTOs.Common;
using CoursePlatform.Application.DTOs.Courses;
using CoursePlatform.Application.DTOs.Lessons;
using CoursePlatform.Application.Interfaces.Services;
using CoursePlatform.Application.Interfaces.UnitOfWork;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Services
{

    public class CourseService(
        IUnitOfWork unitOfWork) : ICourseService
    {
        public async Task<CourseResponse> CreateAsync(
            CreateCourseRequest request,
            string instructorId)
        {
            var course = new Course
            {
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                InstructorId = instructorId,
                Status = CourseStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork
                .Repository<Course>()
                .AddAsync(course);

            await unitOfWork.SaveChangesAsync();

            return new CourseResponse
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                InstructorId = course.InstructorId,
                Status = course.Status.ToString(),
                CreatedAt = course.CreatedAt
            };
        }


        public async Task<PaginatedResponse<CourseWithLessonsResponse>> GetPagedAsync(
            int pageNumber,
            int pageSize)
        {
            var result = await unitOfWork
                .Repository<Course>()
                .GetPagedAsync(
                    pageNumber,
                    pageSize,
                    c => c.Lessons);

            var items = result.Items
                .Select(course => new CourseWithLessonsResponse
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    InstructorId = course.InstructorId,
                    Status = course.Status.ToString(),
                    CreatedAt = course.CreatedAt,

                    Lessons = course.Lessons
                        .OrderBy(l => l.Order)
                        .Select(lesson => new LessonResponse
                        {
                            Id = lesson.Id,
                            CourseId = lesson.CourseId,
                            Title = lesson.Title,
                            Description = lesson.Description,
                            VideoUrl = lesson.VideoUrl,
                            Order = lesson.Order,
                            CreatedAt = lesson.CreatedAt
                        })
                        .ToList()
                })
                .ToList();

            return new PaginatedResponse<CourseWithLessonsResponse>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount,
                TotalPages = (int)Math.Ceiling(
                    result.TotalCount / (double)pageSize)
            };
        }
    }
}
