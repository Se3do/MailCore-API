using FluentValidation;
using MailCore.Application.Commands.Mailbox.MarkStarred;

namespace MailCore.Application.Validators;

public sealed class MarkMailStarredCommandValidator : AbstractValidator<MarkMailStarredCommand>
{
    public MarkMailStarredCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.MailId)
            .NotEmpty().WithMessage("MailId is required.");
    }
}
