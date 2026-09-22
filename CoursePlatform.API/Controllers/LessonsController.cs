using CoursePlatform.API.Extensions;
using CoursePlatform.Application.DTOs.Lessons;
using CoursePlatform.Application.Interfaces.Services;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers
{
    [ApiController]
    [Route("api/courses/{courseId}/lessons")]
    [Authorize(Roles = Roles.Instructor)]
    public class LessonsController(
    ILessonService lessonService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(
            int courseId,
            CreateLessonRequest request)
        {
            var instructorId = User.GetUserId();

            if (instructorId is null)
                return Unauthorized();

            var result = await lessonService.CreateAsync(
                courseId,
                request,
                instructorId);

            if (result.result == LessonActionResult.CourseNotFound)
                return NotFound("Course not found.");

            if (result.result == LessonActionResult.Unauthorized)
                return Forbid();

            if (result.result == LessonActionResult.Failed)
                return BadRequest("Failed to create lesson.");

            return StatusCode(
                StatusCodes.Status201Created,
                result.data);
        }
    }
}
