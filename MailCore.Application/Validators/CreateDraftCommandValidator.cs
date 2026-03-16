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

        RuleForEach(x => x.Request.To!)
            .EmailAddress().WithMessage("To '{PropertyValue}' is not a valid email address.")
            .When(x => x.Request.To is not null && x.Request.To.Count > 0);

        RuleForEach(x => x.Request.Cc!)
            .EmailAddress().WithMessage("CC '{PropertyValue}' is not a valid email address.")
            .When(x => x.Request.Cc is not null && x.Request.Cc.Count > 0);

        RuleForEach(x => x.Request.Bcc!)
            .EmailAddress().WithMessage("BCC '{PropertyValue}' is not a valid email address.")
            .When(x => x.Request.Bcc is not null && x.Request.Bcc.Count > 0);
    }
}
