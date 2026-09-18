using CoursePlatform.Application.DTOs.Auth;
using CoursePlatform.Application.Interfaces.Services;
using CoursePlatform.Application.Interfaces.UnitOfWork;
using CoursePlatform.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CoursePlatform.Infrastructure.Identity;


public class UserService(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork) : IUserService
{
    public async Task<(UserActionResult result, UserResponse? data)> AddAsync(
        CreateUserRequest request,
        string adminId)
    {
        var admin = await userManager.FindByIdAsync(adminId);

        if (admin is null || !await userManager.IsInRoleAsync(admin, Roles.Admin))
        {
            return (UserActionResult.Unauthorized, null);
        }

        var allowedRoles = new[]
        {
            Roles.Admin,
            Roles.Instructor,
            Roles.Student
        };

        if (!allowedRoles.Contains(request.Role))
        {
            return (UserActionResult.InvalidRole, null);
        }

        var existingUser =
            await userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return (UserActionResult.EmailAlreadyExists, null);
        }

        var user = new ApplicationUser
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            UserName = request.Email.Trim(),
            EmailConfirmed = true
        };

        try
        {
            await unitOfWork.ExecuteInTransactionAllContextAsync(async () =>
            {
                var createResult =
                    await userManager.CreateAsync(
                        user,
                        request.Password);

                if (!createResult.Succeeded)
                {
                    return false;
                }

                var roleResult =
                    await userManager.AddToRoleAsync(
                        user,
                        request.Role);

                if (!roleResult.Succeeded)
                {
                    return false;
                }
                return true;

            });
        }
        catch
        {
            return (UserActionResult.Failed, null);
        }

        var createdUser =
            await userManager.FindByIdAsync(user.Id);

        if (createdUser is null)
        {
            return (UserActionResult.Failed, null);
        }

        return (
            UserActionResult.Success,
            new UserResponse
            {
                Id = createdUser.Id,
                FullName = createdUser.FullName,
                Email = createdUser.Email!,
                Role = request.Role
            });
    }
    public async Task<(UserActionResult result, UserResponse? data)> GetByIdAsync(
    string userId,
    string adminId)
    {
        var admin = await userManager.FindByIdAsync(adminId);

        if (admin is null ||
            !admin.IsActive ||
            !await userManager.IsInRoleAsync(admin, Roles.Admin))
        {
            return (UserActionResult.Unauthorized, null);
        }

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return (UserActionResult.NotFound, null);
        }

        var roles = await userManager.GetRolesAsync(user);

        var response = new UserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? string.Empty,
            IsActive = user.IsActive
        };

        return (UserActionResult.Success, response);
    }
    public async Task<UserActionResult> UpdateAsync(
    string userId,
    UpdateUserRequest request,
    string adminId)
    {
        var admin = await userManager.FindByIdAsync(adminId);

        if (admin is null ||
            !admin.IsActive ||
            !await userManager.IsInRoleAsync(admin, Roles.Admin))
        {
            return UserActionResult.Unauthorized;
        }

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return UserActionResult.NotFound;
        }

        var existingUser =
            await userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null &&
            existingUser.Id != user.Id)
        {
            return UserActionResult.EmailAlreadyExists;
        }

        user.FullName = request.FullName.Trim();
        user.Email = request.Email.Trim();
        user.UserName = request.Email.Trim();
        user.IsActive = request.IsActive;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return UserActionResult.Failed;
        }

        return UserActionResult.Success;
    }
    public async Task<UserActionResult> ChangePasswordAsync(
    string userId,
    ChangePasswordRequest request,
    string adminId)
    {
        var admin = await userManager.FindByIdAsync(adminId);

        if (admin is null ||
            !admin.IsActive ||
            !await userManager.IsInRoleAsync(admin, Roles.Admin))
        {
            return UserActionResult.Unauthorized;
        }

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return UserActionResult.NotFound;
        }

        var result = await userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        if (!result.Succeeded)
        {
            return UserActionResult.InvalidPassword;
        }

        return UserActionResult.Success;
    }
}