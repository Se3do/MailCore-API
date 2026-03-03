using FluentValidation;
using MailCore.Application.Commands.Drafts.SendDraft;

namespace MailCore.Application.Validators;

public sealed class SendDraftCommandValidator : AbstractValidator<SendDraftCommand>
{
    public SendDraftCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.DraftId)
            .NotEmpty().WithMessage("DraftId is required.");
    }
}
