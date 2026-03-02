using FluentValidation;
using MailCore.Application.Commands.Drafts.DeleteDraft;

namespace MailCore.Application.Validators;

public sealed class DeleteDraftCommandValidator : AbstractValidator<DeleteDraftCommand>
{
    public DeleteDraftCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.DraftId)
            .NotEmpty().WithMessage("DraftId is required.");
    }
}
