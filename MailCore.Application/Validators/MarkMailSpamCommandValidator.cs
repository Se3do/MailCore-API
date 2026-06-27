using FluentValidation;
using MailCore.Application.Commands.Mailbox.MarkSpam;

namespace MailCore.Application.Validators;

public sealed class MarkMailSpamCommandValidator : AbstractValidator<MarkMailSpamCommand>
{
    public MarkMailSpamCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.MailId)
            .NotEmpty().WithMessage("MailId is required.");
    }
}
