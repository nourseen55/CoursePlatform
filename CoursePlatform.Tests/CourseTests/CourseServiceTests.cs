using CoursePlatform.Application.DTOs.Courses;
using CoursePlatform.Application.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Infrastructure.Persistence.UnitOfWork;
using CoursePlatform.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace CoursePlatform.Tests.CourseTests
{

    public class CourseServiceTests
    {
        [Fact]
        public async Task CreateAsync_ShouldCreateDraftCourse()
        {
            // Arrange
            await using var context = TestDbContextFactory.Create();

            var unitOfWork = new UnitOfWork(context);
            var service = new CourseService(unitOfWork);

            var request = new CreateCourseRequest
            {
                Title = "ASP.NET Core API",
                Description = "Learn how to build APIs."
            };

            var instructorId = "instructor-1";

            // Act
            var data = await service.CreateAsync(
                request,
                instructorId);

            // Assert
            data.Should().NotBeNull();

            data!.Title.Should()
                .Be("ASP.NET Core API");

            data.Description.Should()
                .Be("Learn how to build APIs.");

            data.InstructorId.Should()
                .Be(instructorId);

            data.Status.Should()
                .Be(CourseStatus.Draft.ToString());
        }
        [Fact]
        public async Task PublishAsync_ShouldReturnNoLessons_WhenCourseHasNoLessons()
        {
            // Arrange
            await using var context = TestDbContextFactory.Create();

            var unitOfWork = new UnitOfWork(context);
            var service = new CourseService(unitOfWork);

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

            // Act
            var result = await service.PublishAsync(
                course.Id,
                instructorId);

            // Assert
            result.Should()
                .Be(CourseActionResult.NoLessons);

            course.Status.Should()
                .Be(CourseStatus.Draft);
        }
    }
}
