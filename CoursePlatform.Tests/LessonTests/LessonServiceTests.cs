using CoursePlatform.Application.DTOs.Lessons;
using CoursePlatform.Application.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Infrastructure.Persistence.UnitOfWork;
using CoursePlatform.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace CoursePlatform.Tests.LessonTests;

public class LessonServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateLesson_WhenCourseExistsAndInstructorOwnsIt()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var unitOfWork = new UnitOfWork(context);
        var service = new LessonService(unitOfWork);

        var instructorId = "instructor-1";

        var course = new Course
        {
            Title = "ASP.NET Core API",
            Description = "Learn how to build APIs.",
            InstructorId = instructorId,
            Status = CourseStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var request = new CreateLessonRequest
        {
            Title = "Introduction",
            Description = "Introduction to ASP.NET Core",
            VideoUrl = "https://example.com/video1",
            Order = 1
        };

        // Act
        var (result, data) =
            await service.CreateAsync(
                course.Id,
                request,
                instructorId);

        // Assert
        result.Should()
            .Be(LessonActionResult.Success);

        data.Should()
            .NotBeNull();

        data!.CourseId.Should()
            .Be(course.Id);

        data.Title.Should()
            .Be("Introduction");

        data.Description.Should()
            .Be("Introduction to ASP.NET Core");

        data.VideoUrl.Should()
            .Be("https://example.com/video1");

        data.Order.Should()
            .Be(1);
    }
}