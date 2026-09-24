using CoursePlatform.Application.Interfaces.Repositories;
using CoursePlatform.Application.Interfaces.Services;
using CoursePlatform.Application.Interfaces.UnitOfWork;
using CoursePlatform.Application.Services;
using CoursePlatform.Infrastructure.Identity;
using CoursePlatform.Infrastructure.Persistence.Repositories;
using CoursePlatform.Infrastructure.Persistence.UnitOfWork;

namespace CoursePlatform.API.DependencyInjection
{

    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // Repositories
            services.AddScoped(
                typeof(IGenericRepository<>),
                typeof(GenericRepository<>));

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Application / Business Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ILessonService, LessonService>();

            // JWT
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Identity Seeder
            services.AddScoped<IIdentitySeeder, IdentitySeeder>();

            return services;
        }
    }
}
