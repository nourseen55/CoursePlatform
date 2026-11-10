using System.Text;
using CoursePlatform.API.ExceptionHandling;
using CoursePlatform.API.Filters;
using CoursePlatform.Infrastructure.Identity;
using CoursePlatform.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoursePlatform.API.DependencyInjection
{
    public static class SystemServiceExtensions
    {
        public static IServiceCollection AddSystemServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // MVC / Controllers
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });
            // Swagger
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                    "Bearer",
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Description = "Enter your JWT token."
                    });

                options.AddSecurityRequirement(
                    new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference =
                                new Microsoft.OpenApi.Models.OpenApiReference
                                {
                                    Type =
                                        Microsoft.OpenApi.Models.ReferenceType
                                            .SecurityScheme,

                                    Id = "Bearer"
                                }
                        },

                        Array.Empty<string>()
                    }
                    });
            });

            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString(
                        "DefaultConnection")));

            // Identity
            services
                .AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // JWT Settings
            services.Configure<JwtSettings>(
                configuration.GetSection("JwtSettings"));

            // Authentication
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    var jwtSettings =
                        configuration
                            .GetSection("JwtSettings")
                            .Get<JwtSettings>()
                        ?? throw new InvalidOperationException(
                            "JwtSettings are not configured.");

                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = jwtSettings.Issuer,

                            ValidAudience = jwtSettings.Audience,

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        jwtSettings.Key)),

                            ClockSkew = TimeSpan.Zero
                        };
                });
            services.AddCors(options =>
            {
                options.AddPolicy("Angular", policy =>
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });
            // Authorization
            services.AddAuthorization();

            // Exception Handling
            services.AddProblemDetails();

            services.AddExceptionHandler<
                GlobalExceptionHandler>();

            return services;
        }

    }
}