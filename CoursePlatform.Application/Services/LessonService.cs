using CoursePlatform.Application.DTOs.Lessons;
using CoursePlatform.Application.Interfaces.Services;
using CoursePlatform.Application.Interfaces.UnitOfWork;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Services
{

    public class LessonService(
        IUnitOfWork unitOfWork) : ILessonService
    {
        public async Task<(LessonActionResult result, LessonResponse? data)> CreateAsync(
            int courseId,
            CreateLessonRequest request,
            string instructorId)
        {
            var course = await unitOfWork
                .Repository<Course>()
                .GetByIdAsync(courseId);

            if (course is null)
            {
                return (LessonActionResult.CourseNotFound, null);
            }

            if (course.InstructorId != instructorId)
            {
                return (LessonActionResult.Unauthorized, null);
            }
            var orderTaken = await unitOfWork
            .Repository<Lesson>()
            .FirstOrDefaultAsync(l => l.CourseId == courseId && l.Order == request.Order) is not null;

            if (orderTaken)
                return (LessonActionResult.DuplicateOrder, null);
            var lesson = new Lesson
            {
                CourseId = courseId,
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                VideoUrl = request.VideoUrl.Trim(),
                Order = request.Order,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork
                .Repository<Lesson>()
                .AddAsync(lesson);

            await unitOfWork.SaveChangesAsync();

            var response = new LessonResponse
            {
                Id = lesson.Id,
                CourseId = lesson.CourseId,
                Title = lesson.Title,
                Description = lesson.Description,
                VideoUrl = lesson.VideoUrl,
                Order = lesson.Order,
                CreatedAt = lesson.CreatedAt
            };

            return (LessonActionResult.Success, response);
        }

        public async Task<(LessonActionResult result, LessonResponse? data)> UpdateAsync(
    int courseId,
    int lessonId,
    UpdateLessonRequest request,
    string instructorId)
        {
            var lesson = await unitOfWork
                .Repository<Lesson>()
                .GetByIdAsync(lessonId);

            if (lesson is null || lesson.CourseId != courseId)
                return (LessonActionResult.NotFound, null);

            var course = await unitOfWork
                .Repository<Course>()
                .GetByIdAsync(courseId);

            if (course is null)
                return (LessonActionResult.CourseNotFound, null);

            if (course.InstructorId != instructorId)
                return (LessonActionResult.Unauthorized, null);

            if (course.Status == CourseStatus.Archived)
                return (LessonActionResult.InvalidStatus, null);
            var orderTaken = await unitOfWork
                .Repository<Lesson>()
                .FirstOrDefaultAsync(l =>
                    l.CourseId == courseId &&
                    l.Order == request.Order &&
                    l.Id != lessonId) is not null;

            if (orderTaken)
                return (LessonActionResult.DuplicateOrder, null);

            lesson.Title = request.Title.Trim();
            lesson.Description = request.Description.Trim();
            lesson.VideoUrl = request.VideoUrl.Trim();
            lesson.Order = request.Order;
            lesson.UpdatedAt = DateTime.UtcNow;

            unitOfWork
                .Repository<Lesson>()
                .Update(lesson);

            await unitOfWork.SaveChangesAsync();

            return (LessonActionResult.Success, new LessonResponse
            {
                Id = lesson.Id,
                CourseId = lesson.CourseId,
                Title = lesson.Title,
                Description = lesson.Description,
                VideoUrl = lesson.VideoUrl,
                Order = lesson.Order,
                CreatedAt = lesson.CreatedAt
            });
        }
        public async Task<(LessonActionResult result, LessonResponse? data)> GetByIdAsync(
    int lessonId)
        {
            var lesson = await unitOfWork
                .Repository<Lesson>()
                .GetByIdAsync(lessonId);

            if (lesson is null)
            {
                return (LessonActionResult.NotFound, null);
            }

            var response = new LessonResponse
            {
                Id = lesson.Id,
                CourseId = lesson.CourseId,
                Title = lesson.Title,
                Description = lesson.Description,
                VideoUrl = lesson.VideoUrl,
                Order = lesson.Order,
                CreatedAt = lesson.CreatedAt
            };

            return (LessonActionResult.Success, response);
        }
        public async Task<LessonActionResult> DeleteAsync(
    int lessonId,
    string instructorId)
        {
            var lesson = await unitOfWork
                .Repository<Lesson>()
                .GetByIdAsync(lessonId);

            if (lesson is null)
            {
                return LessonActionResult.NotFound;
            }

            var course = await unitOfWork
                .Repository<Course>()
                .GetByIdAsync(lesson.CourseId);

            if (course is null)
            {
                return LessonActionResult.NotFound;
            }

            if (course.InstructorId != instructorId)
            {
                return LessonActionResult.Unauthorized;
            }
            var hasProgress = await unitOfWork
                .Repository<LessonProgress>()
                .FirstOrDefaultAsync(p => p.LessonId == lessonId) is not null;

            if (hasProgress)
            {
                return LessonActionResult.HasProgress;
            }

            unitOfWork
                .Repository<Lesson>()
                .Delete(lesson);

            await unitOfWork.SaveChangesAsync();

            return LessonActionResult.Success;
        }
    }
}
