using FluentValidation;
using MailCore.Application.Commands.Labels.DeleteLabel;

namespace MailCore.Application.Validators;

public sealed class DeleteLabelCommandValidator : AbstractValidator<DeleteLabelCommand>
{
    public DeleteLabelCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

    RuleFor(x => x.LabelId)
            .NotEmpty().WithMessage("LabelId is required.");
    }
}
