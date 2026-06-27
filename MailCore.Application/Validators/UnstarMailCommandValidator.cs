using FluentValidation;
using MailCore.Application.Commands.Mailbox.Unstar;

namespace MailCore.Application.Validators;

public sealed class UnstarMailCommandValidator : AbstractValidator<UnstarMailCommand>
{
    public UnstarMailCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.MailId)
            .NotEmpty().WithMessage("MailId is required.");
    }
}
