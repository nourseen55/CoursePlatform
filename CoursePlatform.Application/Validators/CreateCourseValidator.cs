using CoursePlatform.Application.DTOs.Courses;
using FluentValidation;

namespace CoursePlatform.Application.Validators
{

    public class CreateCourseValidator : AbstractValidator<CreateCourseRequest>
    {
        public CreateCourseValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

           
        }
    }
    public class UpdateCourseValidator : AbstractValidator<UpdateCourseRequest>
    {
        public UpdateCourseValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        }
    }
}
