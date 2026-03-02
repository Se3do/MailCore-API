using FluentValidation;
using MailCore.Application.Commands.Labels.UnassignLabel;

namespace MailCore.Application.Validators;

public sealed class UnassignLabelCommandValidator : AbstractValidator<UnassignLabelCommand>
{
    public UnassignLabelCommandValidator()
    {
        RuleFor(x => x.userId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.mailId)
            .NotEmpty().WithMessage("MailId is required.");

        RuleFor(x => x.labelId)
            .NotEmpty().WithMessage("LabelId is required.");
    }
}
