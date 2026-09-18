using CoursePlatform.Application.DTOs.Auth;
using CoursePlatform.Application.Interfaces.Services;
using CoursePlatform.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CoursePlatform.Infrastructure.Identity;


public class AuthService(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<(AuthActionResult result, LoginResponse? data)> LoginAsync(
        LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.IsActive)
        {
            return (
                AuthActionResult.InvalidCredentials,
                null);
        }

        var passwordValid =
            await userManager.CheckPasswordAsync(
                user,
                request.Password);

        if (!passwordValid)
        {
            return (
                AuthActionResult.InvalidCredentials,
                null);
        }

        var roles = await userManager.GetRolesAsync(user);

        var tokenResult =
            await jwtTokenService.GenerateTokenAsync(user);

        var response = new LoginResponse
        {
            AccessToken = tokenResult.AccessToken,

            ExpiresAt = tokenResult.ExpiresAt,

            User = new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? string.Empty
            }
        };

        return (
            AuthActionResult.Success,
            response);
    }
}
