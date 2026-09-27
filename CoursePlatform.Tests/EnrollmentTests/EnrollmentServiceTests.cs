using CoursePlatform.Application.DTOs.Enrollments;
using CoursePlatform.Application.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Infrastructure.Persistence.UnitOfWork;
using CoursePlatform.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace CoursePlatform.Tests.EnrollmentTests;

public class EnrollmentServiceTests
{
    [Fact]
    public async Task EnrollAsync_ShouldCreateEnrollment_WhenCourseIsPublished()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var unitOfWork = new UnitOfWork(context);
        var service = new EnrollmentService(unitOfWork);

        var studentId = "student-1";

        var course = new Course
        {
            Title = "ASP.NET Core API",
            Description = "Learn how to build APIs.",
            InstructorId = "instructor-1",
            Status = CourseStatus.Published,
            CreatedAt = DateTime.UtcNow
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync();

        // Act
        var (result, data) = await service.EnrollAsync(
            course.Id,
            studentId);

        // Assert
        result.Should()
            .Be(EnrollmentActionResult.Success);

        data.Should()
            .NotBeNull();

        data!.CourseId.Should()
            .Be(course.Id);

        data.CourseTitle.Should()
            .Be(course.Title);

        data.Status.Should()
            .Be(EnrollmentStatus.Active.ToString());

        data.EnrolledAt.Should()
            .NotBe(default);
    }
}