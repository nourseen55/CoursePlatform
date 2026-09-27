using CoursePlatform.Application.DTOs.Auth;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Infrastructure.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace CoursePlatform.Tests.AuthTests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-1",
            Email = "student@test.com",
            UserName = "student@test.com",
            FullName = "Test Student",
            IsActive = true
        };

        var userManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

        var jwtTokenService = new Mock<IJwtTokenService>();

        userManager
            .Setup(x => x.FindByEmailAsync("student@test.com"))
            .ReturnsAsync(user);

        userManager
            .Setup(x => x.CheckPasswordAsync(
                user,
                "Password123!"))
            .ReturnsAsync(true);

        userManager
            .Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>
            {
                Roles.Student
            });

        jwtTokenService
            .Setup(x => x.GenerateTokenAsync(user))
            .ReturnsAsync(new JwtTokenResult
            {
                AccessToken = "fake-jwt-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });

        var service = new AuthService(
            userManager.Object,
            jwtTokenService.Object);

        var request = new LoginRequest
        {
            Email = "student@test.com",
            Password = "Password123!"
        };

        // Act
        var (result, data) =
            await service.LoginAsync(request);

        // Assert
        result.Should()
            .Be(AuthActionResult.Success);

        data.Should()
            .NotBeNull();

        data!.AccessToken.Should()
            .Be("fake-jwt-token");

        data.User.Email.Should()
            .Be("student@test.com");

        data.User.FullName.Should()
            .Be("Test Student");

        data.User.Role.Should()
            .Be(Roles.Student);
    }
}