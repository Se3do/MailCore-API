using FluentValidation;
using MailCore.Application.Commands.Mailbox.Unspam;

namespace MailCore.Application.Validators;

public sealed class UnspamMailCommandValidator : AbstractValidator<UnspamMailCommand>
{
    public UnspamMailCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.MailId)
            .NotEmpty().WithMessage("MailId is required.");
    }
}
