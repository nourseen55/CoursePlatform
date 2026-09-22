using CoursePlatform.API.Extensions;
using CoursePlatform.Application.DTOs.Courses;
using CoursePlatform.Application.Interfaces.Services;
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
    }
}
