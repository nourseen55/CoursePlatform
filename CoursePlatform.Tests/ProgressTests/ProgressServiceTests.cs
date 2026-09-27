using CoursePlatform.Application.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Infrastructure.Persistence.UnitOfWork;
using CoursePlatform.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoursePlatform.Tests.ProgressTests;

public class ProgressServiceTests
{
    [Fact]
    public async Task CompleteLessonAsync_ShouldCreateProgress_WhenStudentIsEnrolled()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var unitOfWork = new UnitOfWork(context);
        var service = new ProgressService(unitOfWork);

        var studentId = "student-1";

        var course = new Course
        {
            Title = "ASP.NET Core API",
            Description = "Learn how to build APIs.",
            InstructorId = "instructor-1",
            Status = CourseStatus.Published,
            CreatedAt = DateTime.UtcNow
        };

        var lesson = new Lesson
        {
            Title = "Introduction",
            Description = "Introduction to ASP.NET Core",
            VideoUrl = "https://example.com/video1",
            Order = 1,
            Course = course,
            CreatedAt = DateTime.UtcNow
        };

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            Course = course,
            Status = EnrollmentStatus.Active,
            EnrolledAt = DateTime.UtcNow
        };

        context.Courses.Add(course);
        context.Lessons.Add(lesson);
        context.Enrollments.Add(enrollment);

        await context.SaveChangesAsync();

        // Act
        var result = await service.CompleteLessonAsync(
            course.Id,
            lesson.Id,
            studentId);

        // Assert
        result.Should()
            .Be(ProgressActionResult.Success);

        var progress = await context.LessonProgresses
            .FirstOrDefaultAsync(lp =>
                lp.EnrollmentId == enrollment.Id &&
                lp.LessonId == lesson.Id);

        progress.Should().NotBeNull();

        progress!.IsCompleted.Should().BeTrue();

        progress.CompletedAt.Should().NotBeNull();
    }
}