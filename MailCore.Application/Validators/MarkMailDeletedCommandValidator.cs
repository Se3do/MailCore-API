using FluentValidation;
using MailCore.Application.Commands.Mailbox.MarkDeleted;

namespace MailCore.Application.Validators;

public sealed class MarkMailDeletedCommandValidator : AbstractValidator<MarkMailDeletedCommand>
{
    public MarkMailDeletedCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.MailId)
            .NotEmpty().WithMessage("MailId is required.");
    }
}
