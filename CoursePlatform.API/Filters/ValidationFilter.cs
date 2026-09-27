using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoursePlatform.API.Filters
{

    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument is null)
                    continue;

                var validatorType = typeof(IValidator<>)
                    .MakeGenericType(argument.GetType());

                var validator = context.HttpContext
                    .RequestServices
                    .GetService(validatorType) as IValidator;

                if (validator is null)
                    continue;

                var validationContextType = typeof(ValidationContext<>)
                    .MakeGenericType(argument.GetType());

                var validationContext =
                    Activator.CreateInstance(
                        validationContextType,
                        argument) as IValidationContext;

                var result = await validator.ValidateAsync(
                    validationContext!,
                    context.HttpContext.RequestAborted);

                if (!result.IsValid)
                {
                    var errors = result.Errors
                        .GroupBy(error => error.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group
                                .Select(error => error.ErrorMessage)
                                .ToArray());

                    context.Result = new BadRequestObjectResult(
                        new ValidationProblemDetails(errors));

                    return;
                }
            }

            await next();
        }
    }
}
