using FluentValidation;
using MailCore.Application.Commands.Labels.UnassignLabel;

namespace MailCore.Application.Validators;

public sealed class UnassignLabelCommandValidator : AbstractValidator<UnassignLabelCommand>
{
    public UnassignLabelCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.MailId)
            .NotEmpty().WithMessage("MailId is required.");

        RuleFor(x => x.LabelId)
            .NotEmpty().WithMessage("LabelId is required.");
    }
}
