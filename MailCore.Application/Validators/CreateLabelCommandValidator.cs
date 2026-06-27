using FluentValidation;
using MailCore.Application.Commands.Labels.CreateLabel;

namespace MailCore.Application.Validators;

public sealed class CreateLabelCommandValidator : AbstractValidator<CreateLabelCommand>
{
    public CreateLabelCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Label name is required.")
            .MaximumLength(100).WithMessage("Label name must not exceed 100 characters.");

        RuleFor(x => x.Request.Color)
            .Matches(@"^#[0-9A-Fa-f]{6}$")
            .WithMessage("Color must be a valid hex color (e.g. #FF5733).")
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Color));
    }
}
