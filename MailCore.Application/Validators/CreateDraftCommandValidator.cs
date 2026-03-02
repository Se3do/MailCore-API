using FluentValidation;
using MailCore.Application.Commands.Drafts.CreateDraft;

namespace MailCore.Application.Validators;

public sealed class CreateDraftCommandValidator : AbstractValidator<CreateDraftCommand>
{
    public CreateDraftCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Request.Subject)
            .MaximumLength(500).WithMessage("Subject must not exceed 500 characters.");

        RuleFor(x => x.Request.Body)
            .NotEmpty().WithMessage("Body is required.");
    }
}
