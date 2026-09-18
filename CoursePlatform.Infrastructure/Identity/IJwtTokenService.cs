using CoursePlatform.Application.DTOs.Auth;

namespace CoursePlatform.Infrastructure.Identity
{
    public interface IJwtTokenService
    {
        Task<JwtTokenResult> GenerateTokenAsync(
            ApplicationUser user);
    }
}
