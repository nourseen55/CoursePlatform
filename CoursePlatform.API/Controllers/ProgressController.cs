using CoursePlatform.API.Extensions;
using CoursePlatform.Application.Interfaces.Services;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Student)]
    public class ProgressController(IProgressService progressService) : ControllerBase
    {
        [HttpPost("{courseId:int}/lessons/{lessonId:int}/complete")]
        public async Task<IActionResult> CompleteLesson(
      int courseId,
      int lessonId)
        {
            var studentId = User.GetUserId();

            if (studentId is null)
                return Unauthorized();

            var result = await progressService.CompleteLessonAsync(
                courseId,
                lessonId,
                studentId);

            return result switch
            {
                ProgressActionResult.Success
                    => NoContent(),

                ProgressActionResult.CourseNotFound
                    => NotFound("Course not found."),

                ProgressActionResult.LessonNotFound
                    => NotFound("Lesson not found."),

                ProgressActionResult.NotEnrolled
                    => Forbid(),

                ProgressActionResult.InvalidLesson
                    => BadRequest(
                        "The lesson does not belong to this course."),

                _ => BadRequest("Failed to complete lesson.")
            };
        }

        [HttpGet("{courseId:int}/progress")]
        public async Task<IActionResult> GetCourseProgress(
            int courseId)
        {
            var studentId = User.GetUserId();

            if (studentId is null)
                return Unauthorized();

            var (result, data) =
                await progressService.GetCourseProgressAsync(
                    courseId,
                    studentId);

            return result switch
            {
                ProgressActionResult.Success
                    => Ok(data),

                ProgressActionResult.CourseNotFound
                    => NotFound("Course not found."),

                ProgressActionResult.NotEnrolled
                    => Forbid(),

                _ => BadRequest("Failed to get course progress.")
            };
        }
    }
}
