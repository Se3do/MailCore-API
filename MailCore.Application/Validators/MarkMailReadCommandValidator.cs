using FluentValidation;
using MailCore.Application.Commands.Mailbox.MarkRead;

namespace MailCore.Application.Validators;

public sealed class MarkMailReadCommandValidator : AbstractValidator<MarkMailReadCommand>
{
    public MarkMailReadCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.MailId)
            .NotEmpty().WithMessage("MailId is required.");
    }
}
