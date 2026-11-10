using CoursePlatform.API.Extensions;
using CoursePlatform.Application.DTOs.Courses;
using CoursePlatform.Application.Interfaces.Services;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers
{

    [ApiController]
    [Route("api/courses")]
    [Authorize(Roles = Roles.Instructor)]
    public class CoursesController(
        ICourseService courseService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateCourseRequest request)
        {
            var instructorId = User.GetUserId();

            if (instructorId is null)
                return Unauthorized();

            var result = await courseService.CreateAsync(
                request,
                instructorId);

            return StatusCode(
             StatusCodes.Status201Created,
             result);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCourses(
        int pageNumber = 1,
        int pageSize = 10)
        {
            if (pageNumber < 1)
                return BadRequest("Page number must be greater than 0.");

            if (pageSize < 1 || pageSize > 100)
                return BadRequest(
                    "Page size must be between 1 and 100.");

            var result = await courseService.GetPagedAsync(
                pageNumber,
                pageSize);

            return Ok(result);
        }
        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await courseService.GetByIdAsync(id);

            if (result.result == CourseActionResult.NotFound)
                return NotFound("Course not found.");

            return Ok(result.data);
        }
        [Authorize(Roles = Roles.Instructor)]
        [HttpPost("{id:int}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            var instructorId = User.GetUserId();

            if (instructorId is null)
                return Unauthorized();

            var result = await courseService.PublishAsync(
                id,
                instructorId);

            if (result == CourseActionResult.NotFound)
                return NotFound("Course not found.");

            if (result == CourseActionResult.Unauthorized)
                return Forbid();

            if (result == CourseActionResult.NoLessons)
                return BadRequest(
                    "Course must contain at least one lesson before publishing.");

            if (result == CourseActionResult.InvalidStatus)
                return BadRequest(
                    "Only draft courses can be published.");

            if (result == CourseActionResult.Failed)
                return BadRequest("Failed to publish course.");

            return NoContent();
        }
        [Authorize(Roles = Roles.Instructor)]
        [HttpPost("{id:int}/archive")]
        public async Task<IActionResult> Archive(int id)
        {
            var instructorId = User.GetUserId();

            if (instructorId is null)
                return Unauthorized();

            var result = await courseService.ArchiveAsync(
                id,
                instructorId);

            if (result == CourseActionResult.NotFound)
                return NotFound("Course not found.");

            if (result == CourseActionResult.Unauthorized)
                return Forbid();

            if (result == CourseActionResult.InvalidStatus)
                return BadRequest(
                    "Only published courses can be archived.");

            if (result == CourseActionResult.Failed)
                return BadRequest("Failed to archive course.");

            return NoContent();
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var instructorId = User.GetUserId();

            if (instructorId is null)
                return Unauthorized();

            var result = await courseService.GetByInstructorAsync(instructorId);

            return Ok(result);
        } 

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateCourseRequest request)
        {
            var instructorId = User.GetUserId();

            if (instructorId is null)
                return Unauthorized();

            var result = await courseService.UpdateAsync(
                id,
                request,
                instructorId);

            if (result.result == CourseActionResult.NotFound)
                return NotFound("Course not found.");

            if (result.result == CourseActionResult.Unauthorized)
                return Forbid();

            if (result.result == CourseActionResult.InvalidStatus)
                return BadRequest("Archived courses cannot be edited.");

            if (result.result == CourseActionResult.Failed)
                return BadRequest("Failed to update course.");

            return Ok(result.data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var instructorId = User.GetUserId();

            if (instructorId is null)
                return Unauthorized();

            var result = await courseService.DeleteAsync(id, instructorId);

            if (result == CourseActionResult.NotFound)
                return NotFound("Course not found.");

            if (result == CourseActionResult.Unauthorized)
                return Forbid();

            if (result == CourseActionResult.InvalidStatus)
                return BadRequest(
                    "Only draft courses can be deleted. Archive the course instead.");

            return NoContent();
        }

    }
}
