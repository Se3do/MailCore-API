using FluentValidation;
using MailCore.Application.Commands.Mailbox.MarkUnread;

namespace MailCore.Application.Validators;

public sealed class MarkMailUnreadCommandValidator : AbstractValidator<MarkMailUnreadCommand>
{
    public MarkMailUnreadCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.MailId)
            .NotEmpty().WithMessage("MailId is required.");
    }
}
