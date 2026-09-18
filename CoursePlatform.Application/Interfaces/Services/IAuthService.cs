using CoursePlatform.Application.DTOs.Auth;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Interfaces.Services;

public interface IAuthService
{
    Task<(AuthActionResult result, LoginResponse? data)> LoginAsync(
        LoginRequest request);
}