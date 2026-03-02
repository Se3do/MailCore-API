using FluentValidation;
using MailCore.Application.Commands.Drafts.UpdateDraft;

namespace MailCore.Application.Validators;

public sealed class UpdateDraftCommandValidator : AbstractValidator<UpdateDraftCommand>
{
    public UpdateDraftCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.DraftId)
            .NotEmpty().WithMessage("DraftId is required.");

        RuleFor(x => x.Request.Subject)
            .MaximumLength(500).WithMessage("Subject must not exceed 500 characters.");

      RuleFor(x => x.Request.Body)
            .NotEmpty().WithMessage("Body is required.");
    }
}
