namespace CoursePlatform.Domain.Enums;

public enum UserActionResult
{
    Success,
    NotFound,
    Failed,
    InvalidRole,
    EmailAlreadyExists,
    InvalidPassword,
    Unauthorized,
}