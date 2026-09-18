using CoursePlatform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CoursePlatform.Infrastructure.Identity;

public class IdentitySeeder(
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext dbContext,
    IConfiguration configuration) : IIdentitySeeder
{
    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[]
        {
            Roles.Admin,
            Roles.Instructor,
            Roles.Student
        };

        foreach (var role in roles)
        {
            if (await roleManager.RoleExistsAsync(role))
                continue;

            var result = await roleManager.CreateAsync(
                new IdentityRole(role));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to seed role '{role}': {errors}");
            }
        }
    }

    private async Task SeedAdminAsync()
    {
        var adminEmail =
            configuration["AdminSettings:Email"]
            ?? throw new InvalidOperationException(
                "Admin email is not configured.");

        var adminPassword =
            configuration["AdminSettings:Password"]
            ?? throw new InvalidOperationException(
                "Admin password is not configured.");

        var adminFullName =
            configuration["AdminSettings:FullName"]
            ?? "System Admin";

        var existingAdmin =
            await userManager.FindByEmailAsync(adminEmail);

        if (existingAdmin is not null)
        {
            if (!await userManager.IsInRoleAsync(
                    existingAdmin,
                    Roles.Admin))
            {
                var roleResult = await userManager.AddToRoleAsync(
                    existingAdmin,
                    Roles.Admin);

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        roleResult.Errors.Select(e => e.Description));

                    throw new InvalidOperationException(
                        $"Failed to assign Admin role to '{adminEmail}': {errors}");
                }
            }

            return;
        }

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync();

        try
        {
            var admin = new ApplicationUser
            {
                FullName = adminFullName,
                Email = adminEmail,
                UserName = adminEmail,
                EmailConfirmed = true
            };

            var createResult =
                await userManager.CreateAsync(
                    admin,
                    adminPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    createResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to seed admin '{adminEmail}': {errors}");
            }

            var roleResult =
                await userManager.AddToRoleAsync(
                    admin,
                    Roles.Admin);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to assign Admin role to '{adminEmail}': {errors}");
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            throw new InvalidOperationException(
                $"Failed to seed admin '{adminEmail}'. " +
                $"Error: {ex.InnerException?.Message ?? ex.Message}",
                ex);
        }
    }
}