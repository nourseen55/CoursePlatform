using CoursePlatform.Application.DTOs.Lessons;
using CoursePlatform.Application.Interfaces.Services;
using CoursePlatform.Application.Interfaces.UnitOfWork;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Services;

public class ProgressService(
    IUnitOfWork unitOfWork) : IProgressService
{
    public async Task<ProgressActionResult> CompleteLessonAsync(
          int courseId,
          int lessonId,
          string studentId)
    {
        var course = await unitOfWork
            .Repository<Course>()
            .GetByIdAsync(courseId);

        if (course is null)
            return ProgressActionResult.CourseNotFound;

        var enrollment = await unitOfWork
            .Repository<Enrollment>()
            .FirstOrDefaultAsync(e =>
                e.StudentId == studentId &&
                e.CourseId == courseId);

        if (enrollment is null)
            return ProgressActionResult.NotEnrolled;

        var lesson = await unitOfWork
            .Repository<Lesson>()
            .GetByIdAsync(lessonId);

        if (lesson is null)
            return ProgressActionResult.LessonNotFound;

        if (lesson.CourseId != courseId)
            return ProgressActionResult.InvalidLesson;

        var progress = await unitOfWork
            .Repository<LessonProgress>()
            .FirstOrDefaultAsync(lp =>
                lp.EnrollmentId == enrollment.Id &&
                lp.LessonId == lessonId);

        if (progress is null)
        {
            progress = new LessonProgress
            {
                EnrollmentId = enrollment.Id,
                LessonId = lessonId,
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow
            };

            await unitOfWork
                .Repository<LessonProgress>()
                .AddAsync(progress);
        }
        else
        {
            progress.IsCompleted = true;
            progress.CompletedAt = DateTime.UtcNow;

            unitOfWork
                .Repository<LessonProgress>()
                .Update(progress);
        }

        await unitOfWork.SaveChangesAsync();

        await CheckCourseCompletionAsync(
            courseId,
            enrollment.Id);

        return ProgressActionResult.Success;
    }
    private async Task CheckCourseCompletionAsync(
        int courseId,
        int enrollmentId)
    {
        var lessons = await unitOfWork
            .Repository<Lesson>()
            .GetAllAsync(l =>
                l.CourseId == courseId);

        if (lessons.Count == 0)
            return;

        var completedLessons = await unitOfWork
            .Repository<LessonProgress>()
            .GetAllAsync(lp =>
                lp.EnrollmentId == enrollmentId &&
                lp.IsCompleted);

        if (completedLessons.Count != lessons.Count)
            return;

        var enrollment = await unitOfWork
            .Repository<Enrollment>()
            .GetByIdAsync(enrollmentId);

        if (enrollment is null)
            return;

        if (enrollment.Status == EnrollmentStatus.Completed)
            return;

        enrollment.Status = EnrollmentStatus.Completed;
        enrollment.UpdatedAt = DateTime.UtcNow;

        unitOfWork
            .Repository<Enrollment>()
            .Update(enrollment);

        await unitOfWork.SaveChangesAsync();
    }
    public async Task<(
        ProgressActionResult result,
        CourseProgressResponse? data)>
        GetCourseProgressAsync(
            int courseId,
            string studentId)
    {
        var course = await unitOfWork
            .Repository<Course>()
            .GetByIdAsync(courseId);

        if (course is null)
            return (
                ProgressActionResult.CourseNotFound,
                null);

        var enrollment = await unitOfWork
            .Repository<Enrollment>()
            .FirstOrDefaultAsync(e =>
                e.StudentId == studentId &&
                e.CourseId == courseId);

        if (enrollment is null)
            return (
                ProgressActionResult.NotEnrolled,
                null);

        var lessons = await unitOfWork
            .Repository<Lesson>()
            .GetAllAsync(l =>
                l.CourseId == courseId);

        var progressRecords = await unitOfWork
            .Repository<LessonProgress>()
            .GetAllAsync(lp =>
                lp.EnrollmentId == enrollment.Id);

        var totalLessons = lessons.Count;

        var completedLessons = progressRecords
            .Count(p => p.IsCompleted);

        decimal percentage = totalLessons == 0
            ? 0
            : (decimal)completedLessons
                / totalLessons * 100;

        var response = new CourseProgressResponse
        {
            CourseId = courseId,
            TotalLessons = totalLessons,
            CompletedLessons = completedLessons,
            ProgressPercentage = percentage
        };

        return (
            ProgressActionResult.Success,
            response);
    }
}