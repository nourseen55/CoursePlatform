using CoursePlatform.Application.DTOs.Auth;
using CoursePlatform.Application.Interfaces.Services;
using CoursePlatform.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(
    IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequest request)
        {
            var result = await authService.LoginAsync(request);

            if (result.result == AuthActionResult.InvalidCredentials)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            if (result.result == AuthActionResult.Failed)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = "An unexpected error occurred."
                    });
            }

            return Ok(result.data);
        }
    }
}
