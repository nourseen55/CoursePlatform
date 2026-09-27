using CoursePlatform.API.Extensions;
using CoursePlatform.Application.Interfaces.Services;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers
{

    [ApiController]
    [Route("api/courses")]
    [Authorize(Roles = Roles.Student)]
    public class EnrollmentsController(
        IEnrollmentService enrollmentService)
        : ControllerBase
    {
        [HttpPost("{courseId:int}/enroll")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var studentId = User.GetUserId();

            if (studentId is null)
                return Unauthorized();

            var (result, data) =
                await enrollmentService.EnrollAsync(
                    courseId,
                    studentId);

            if (result == EnrollmentActionResult.CourseNotFound)
                return NotFound("Course not found.");

            if (result == EnrollmentActionResult.NotPublished)
                return BadRequest(
                    "You can only enroll in a published course.");

            if (result == EnrollmentActionResult.AlreadyEnrolled)
                return Conflict(
                    "You are already enrolled in this course.");

            if (result == EnrollmentActionResult.Failed)
                return BadRequest(
                    "Enrollment failed.");

            return Ok(data);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var studentId = User.GetUserId();

            if (studentId is null)
                return Unauthorized();

            var result = await enrollmentService
                .GetMyEnrollmentsAsync(studentId);

            return Ok(result);
        }
    }
}
