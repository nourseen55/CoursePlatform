using CoursePlatform.Application.DTOs.Auth;
using FluentValidation;

namespace CoursePlatform.Application.Validators
{
    public class CreateUserValidator
     : AbstractValidator<CreateUserRequest>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8);

            RuleFor(x => x.Role)
                .NotEmpty();
        }
    }

}