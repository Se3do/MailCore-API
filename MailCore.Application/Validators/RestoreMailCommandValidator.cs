using FluentValidation;
using MailCore.Application.Commands.Mailbox.Restore;

namespace MailCore.Application.Validators;

public sealed class RestoreMailCommandValidator : AbstractValidator<RestoreMailCommand>
{
    public RestoreMailCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.MailId)
            .NotEmpty().WithMessage("MailId is required.");
    }
}
