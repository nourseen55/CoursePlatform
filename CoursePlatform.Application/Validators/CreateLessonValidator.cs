using CoursePlatform.Application.DTOs.Lessons;
using FluentValidation;

namespace CoursePlatform.Application.Validators
{
    public class CreateLessonValidator : AbstractValidator<CreateLessonRequest>
    {
        public CreateLessonValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.VideoUrl).NotEmpty();
            //RuleFor(x => x.VideoUrl)
            //  .NotEmpty()
            //  .MaximumLength(500)
            //  .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri)
            //      && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            //  .WithMessage("VideoUrl must be a valid http or https URL.");

            RuleFor(x => x.Order).GreaterThan(0);
        }
    }

    public class UpdateLessonValidator : AbstractValidator<UpdateLessonRequest>
    {
        public UpdateLessonValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);

            //RuleFor(x => x.VideoUrl)
            //    .NotEmpty()
            //    .MaximumLength(500)
            //    .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri)
            //        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            //    .WithMessage("VideoUrl must be a valid http or https URL.");

            RuleFor(x => x.Order).GreaterThan(0);
        }
    }
}
