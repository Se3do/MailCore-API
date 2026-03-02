using FluentValidation;
using MailCore.Application.Commands.Labels.AssignLabel;

namespace MailCore.Application.Validators;

public sealed class AssignLabelCommandValidator : AbstractValidator<AssignLabelCommand>
{
    public AssignLabelCommandValidator()
    {
        RuleFor(x => x.userId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.mailId)
            .NotEmpty().WithMessage("MailId is required.");

        RuleFor(x => x.labelId)
            .NotEmpty().WithMessage("LabelId is required.");
    }
}
