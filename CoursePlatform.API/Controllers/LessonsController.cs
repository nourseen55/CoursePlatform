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
            if (result.result == LessonActionResult.DuplicateOrder)
                return Conflict("Another lesson in this course already uses this order.");

            if (result.result == LessonActionResult.Failed)
                return BadRequest("Failed to create lesson.");

            return StatusCode(
                StatusCodes.Status201Created,
                result.data);
        }

        [HttpPut("{lessonId:int}")]
        public async Task<IActionResult> Update(
    int courseId,
    int lessonId,
    UpdateLessonRequest request)
        {
            var instructorId = User.GetUserId();

            if (instructorId is null)
                return Unauthorized();

            var result = await lessonService.UpdateAsync(
                courseId,
                lessonId,
                request,
                instructorId);

            if (result.result == LessonActionResult.NotFound)
                return NotFound("Lesson not found.");

            if (result.result == LessonActionResult.CourseNotFound)
                return NotFound("Course not found.");

            if (result.result == LessonActionResult.Unauthorized)
                return Forbid();
            if (result.result == LessonActionResult.DuplicateOrder)
                return Conflict("Another lesson in this course already uses this order.");

            if (result.result == LessonActionResult.InvalidStatus)
                return BadRequest("Archived courses cannot be edited.");

            return Ok(result.data);

        }


        [AllowAnonymous]
        [HttpGet("{lessonId:int}")]
        public async Task<IActionResult> GetById(int lessonId)
        {
            var result = await lessonService.GetByIdAsync(lessonId);

            if (result.result == LessonActionResult.NotFound)
                return NotFound("Lesson not found.");

            return Ok(result.data);
        }
        [Authorize(Roles = Roles.Instructor)]
        [HttpDelete("{lessonId:int}")]
        public async Task<IActionResult> Delete(int lessonId)
        {
            var instructorId = User.GetUserId();

            if (instructorId is null)
                return Unauthorized();

            var result = await lessonService.DeleteAsync(
                lessonId,
                instructorId);

            if (result == LessonActionResult.NotFound)
                return NotFound("Lesson not found.");

            if (result == LessonActionResult.Unauthorized)
                return Forbid();

            if (result == LessonActionResult.HasProgress)
                return Conflict("This lesson has student progress and cannot be deleted.");

            if (result == LessonActionResult.Failed)
                return BadRequest("Failed to delete lesson.");

            return NoContent();
        }
    }
}
