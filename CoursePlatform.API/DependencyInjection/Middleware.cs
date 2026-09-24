namespace CoursePlatform.API.DependencyInjection
{
    public static class MiddlewareExtensions
    {
        public static WebApplication UseApplicationMiddleware(
            this WebApplication app)
        {
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}
