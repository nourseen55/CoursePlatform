using CoursePlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CoursePlatform.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<Course> Courses { get; set; }
        = new List<Course>();

    public ICollection<Enrollment> Enrollments { get; set; }
        = new List<Enrollment>();
}