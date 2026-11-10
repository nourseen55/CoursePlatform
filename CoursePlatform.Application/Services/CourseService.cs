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
                    pageSize, c => c.Status == CourseStatus.Published,
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
        public async Task<(
    CourseActionResult result,
    CourseWithLessonsResponse? data)> GetByIdAsync(
        int courseId)
        {
            var course = await unitOfWork
                .Repository<Course>()
                .GetByIdAsync(
                    courseId,
                    c => c.Lessons);

            if (course is null)
            {
                return (
                    CourseActionResult.NotFound,
                    null);
            }

            var response = new CourseWithLessonsResponse
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                InstructorId = course.InstructorId,
                Status = course.Status.ToString(),
                CreatedAt = course.CreatedAt,

                Lessons = course.Lessons
                    .OrderBy(l => l.Order)
                    .Select(l => new LessonResponse
                    {
                        Id = l.Id,
                        CourseId = l.CourseId,
                        Title = l.Title,
                        Description = l.Description,
                        VideoUrl = l.VideoUrl,
                        Order = l.Order,
                        CreatedAt = l.CreatedAt
                    })
                    .ToList()
            };

            return (
                CourseActionResult.Success,
                response);
        }

        public async Task<IReadOnlyList<InstructorCourseResponse>> GetByInstructorAsync(
    string instructorId)
        {
            var courses = await unitOfWork
                .Repository<Course>()
                .GetAllAsync(c => c.InstructorId == instructorId, c => c.Lessons);

            return courses
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new InstructorCourseResponse
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    InstructorId = c.InstructorId,
                    Status = c.Status.ToString(),
                    CreatedAt = c.CreatedAt,
                    LessonsCount = c.Lessons.Count
                })
                .ToList();
        }

        public async Task<(CourseActionResult result, CourseResponse? data)> UpdateAsync(
            int courseId,
            UpdateCourseRequest request,
            string instructorId)
        {
            var course = await unitOfWork
                .Repository<Course>()
                .GetByIdAsync(courseId);

            if (course is null)
                return (CourseActionResult.NotFound, null);

            if (course.InstructorId != instructorId)
                return (CourseActionResult.Unauthorized, null);

            if (course.Status == CourseStatus.Archived)
                return (CourseActionResult.InvalidStatus, null);

            course.Title = request.Title.Trim();
            course.Description = request.Description.Trim();
            course.UpdatedAt = DateTime.UtcNow;

            unitOfWork
                .Repository<Course>()
                .Update(course);

            await unitOfWork.SaveChangesAsync();

            return (CourseActionResult.Success, new CourseResponse
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                InstructorId = course.InstructorId,
                Status = course.Status.ToString(),
                CreatedAt = course.CreatedAt
            });
        }

        public async Task<CourseActionResult> DeleteAsync(
            int courseId,
            string instructorId)
        {
            var course = await unitOfWork
                .Repository<Course>()
                .GetByIdAsync(courseId);

            if (course is null)
                return CourseActionResult.NotFound;

            if (course.InstructorId != instructorId)
                return CourseActionResult.Unauthorized;

            if (course.Status != CourseStatus.Draft)
                return CourseActionResult.InvalidStatus;

            var hasEnrollments = await unitOfWork
                .Repository<Enrollment>()
                .FirstOrDefaultAsync(e => e.CourseId == courseId) is not null;

            if (hasEnrollments)
                return CourseActionResult.InvalidStatus;

            unitOfWork
                .Repository<Course>()
                .Delete(course);

            await unitOfWork.SaveChangesAsync();

            return CourseActionResult.Success;
        }


        public async Task<CourseActionResult> PublishAsync(
    int courseId,
    string instructorId)
        {
            var course = await unitOfWork
                .Repository<Course>()
                .GetByIdAsync(
                    courseId,
                    c => c.Lessons);

            if (course is null)
            {
                return CourseActionResult.NotFound;
            }

            if (course.InstructorId != instructorId)
            {
                return CourseActionResult.Unauthorized;
            }

            if (course.Status != CourseStatus.Draft)
            {
                return CourseActionResult.InvalidStatus;
            }

            if (!course.Lessons.Any())
            {
                return CourseActionResult.NoLessons;
            }

            course.Status = CourseStatus.Published;
            course.UpdatedAt = DateTime.UtcNow;

            unitOfWork
                .Repository<Course>()
                .Update(course);

            await unitOfWork.SaveChangesAsync();

            return CourseActionResult.Success;
        }
        public async Task<CourseActionResult> ArchiveAsync(
    int courseId,
    string instructorId)
        {
            var course = await unitOfWork
                .Repository<Course>()
                .GetByIdAsync(courseId);

            if (course is null)
            {
                return CourseActionResult.NotFound;
            }

            if (course.InstructorId != instructorId)
            {
                return CourseActionResult.Unauthorized;
            }

            if (course.Status != CourseStatus.Published)
            {
                return CourseActionResult.InvalidStatus;
            }

            course.Status = CourseStatus.Archived;
            course.UpdatedAt = DateTime.UtcNow;

            unitOfWork
                .Repository<Course>()
                .Update(course);

            await unitOfWork.SaveChangesAsync();

            return CourseActionResult.Success;
        }
        public async Task<(
    CourseAccessResult result,
    CourseWithLessonsResponse? data)>
    GetForStudentAsync(
        int courseId,
        string studentId)
        {
            var course = await unitOfWork
                .Repository<Course>()
                .GetByIdAsync(
                    courseId,
                    c => c.Lessons);

            if (course is null)
                return (
                    CourseAccessResult.CourseNotFound,
                    null);


            var enrollment =
                 unitOfWork.Repository<Enrollment>()
                    .GetAllQuerable().FirstOrDefault(e => e.CourseId == courseId && e.StudentId == studentId);


            if (enrollment is null)
                return (
                    CourseAccessResult.NotEnrolled,
                    null);

            var response = new CourseWithLessonsResponse
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                InstructorId = course.InstructorId,
                Status = course.Status.ToString(),
                CreatedAt = course.CreatedAt,

                Lessons = course.Lessons
                    .OrderBy(l => l.Order)
                    .Select(l => new LessonResponse
                    {
                        Id = l.Id,
                        CourseId = l.CourseId,
                        Title = l.Title,
                        Description = l.Description,
                        VideoUrl = l.VideoUrl,
                        Order = l.Order,
                        CreatedAt = l.CreatedAt
                    })
                    .ToList()
            };

            return (
                CourseAccessResult.Success,
                response);
        }
    }
}
