using CoursePlatform.API.Extensions;
using CoursePlatform.Application.DTOs.Auth;
using CoursePlatform.Application.Interfaces.Services;
using CoursePlatform.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController(IUserService _userService) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> Add(
        CreateUserRequest request,
        CancellationToken cancellationToken)
        {
            var adminId = User.GetUserId();

            if (adminId is null)
                return Unauthorized();

            var result = await _userService.AddAsync(
                request,
                adminId);

            if (result.result == UserActionResult.Unauthorized)
                return Forbid();

            if (result.result == UserActionResult.InvalidRole)
                return BadRequest("Invalid user type.");

            if (result.result == UserActionResult.EmailAlreadyExists)
                return Conflict("Email already exists.");

            if (result.result == UserActionResult.Failed)
                return BadRequest("Failed to create user.");

            return StatusCode(
                StatusCodes.Status201Created,
                result.data);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var adminId = User.GetUserId();

            if (adminId is null)
                return Unauthorized();

            var result = await _userService.GetByIdAsync(
                id,
                adminId);

            if (result.result == UserActionResult.Unauthorized)
                return Forbid();

            if (result.result == UserActionResult.NotFound)
                return NotFound();

            return Ok(result.data);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
    string id,
    UpdateUserRequest request)
        {
            var adminId = User.GetUserId();

            if (adminId is null)
                return Unauthorized();

            var result = await _userService.UpdateAsync(
                id,
                request,
                adminId);

            if (result == UserActionResult.Unauthorized)
                return Forbid();

            if (result == UserActionResult.NotFound)
                return NotFound();

            if (result == UserActionResult.EmailAlreadyExists)
                return Conflict("Email already exists.");

            if (result == UserActionResult.Failed)
                return BadRequest("Failed to update user.");

            return NoContent();
        }
        [HttpPatch("{id}/password")]
        public async Task<IActionResult> ChangePassword(
    string id,
    ChangePasswordRequest request)
        {
            var adminId = User.GetUserId();

            if (adminId is null)
                return Unauthorized();

            var result = await _userService.ChangePasswordAsync(
                id,
                request,
                adminId);

            if (result == UserActionResult.Unauthorized)
                return Forbid();

            if (result == UserActionResult.NotFound)
                return NotFound();

            if (result == UserActionResult.InvalidPassword)
            {
                return BadRequest(
                    "Invalid current password or new password.");
            }

            if (result == UserActionResult.Failed)
            {
                return BadRequest(
                    "Failed to change password.");
            }

            return NoContent();
        }
    }
}
