using FluentValidation;
using MailCore.Application.Commands.Labels.AssignLabel;

namespace MailCore.Application.Validators;

public sealed class AssignLabelCommandValidator : AbstractValidator<AssignLabelCommand>
{
    public AssignLabelCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.MailId)
            .NotEmpty().WithMessage("MailId is required.");

        RuleFor(x => x.LabelId)
            .NotEmpty().WithMessage("LabelId is required.");
    }
}
