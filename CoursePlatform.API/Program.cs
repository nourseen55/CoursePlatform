using CoursePlatform.API.DependencyInjection;
using CoursePlatform.Infrastructure.Identity;

namespace CoursePlatform.API
{

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // System / Framework services
            builder.Services.AddSystemServices(
                builder.Configuration);

            // Application services
            builder.Services.AddApplicationServices();

            var app = builder.Build();

            // Seed Identity
            using (var scope = app.Services.CreateScope())
            {
                var seeder =
                    scope.ServiceProvider
                        .GetRequiredService<IIdentitySeeder>();

                await seeder.SeedAsync();
            }

            // HTTP pipeline
            app.UseApplicationMiddleware();

            app.Run();
        }
    }
}
