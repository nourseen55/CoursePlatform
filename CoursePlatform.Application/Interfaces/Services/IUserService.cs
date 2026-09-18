using CoursePlatform.Application.DTOs.Auth;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.Interfaces.Services;

public interface IUserService
{
    Task<(UserActionResult result, UserResponse? data)> AddAsync(
        CreateUserRequest request,
        string adminId);

    Task<(UserActionResult result, UserResponse? data)> GetByIdAsync(
        string userId,
        string adminId);

    Task<UserActionResult> UpdateAsync(
        string userId,
        UpdateUserRequest request,
        string adminId);
    Task<UserActionResult> ChangePasswordAsync(
    string userId,
    ChangePasswordRequest request,
    string adminId);
}