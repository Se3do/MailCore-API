using FluentValidation;
using MailCore.Application.Commands.Emails.ForwardEmail;

namespace MailCore.Application.Validators;

public sealed class ForwardEmailCommandValidator : AbstractValidator<ForwardEmailCommand>
{
    public ForwardEmailCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.EmailId)
            .NotEmpty().WithMessage("EmailId is required.");

        RuleFor(x => x.Request.Body)
            .NotEmpty().WithMessage("Body is required.");

        RuleFor(x => x.Request.To)
            .NotEmpty().WithMessage("At least one recipient (To) is required.");

        RuleForEach(x => x.Request.To)
            .NotEmpty().WithMessage("Recipient email must not be empty.")
            .EmailAddress().WithMessage("'{PropertyValue}' is not a valid email address.");

        RuleForEach(x => x.Request.Cc!)
            .EmailAddress().WithMessage("CC '{PropertyValue}' is not a valid email address.")
            .When(x => x.Request.Cc is not null && x.Request.Cc.Count > 0);

        RuleForEach(x => x.Request.Bcc!)
            .EmailAddress().WithMessage("BCC '{PropertyValue}' is not a valid email address.")
            .When(x => x.Request.Bcc is not null && x.Request.Bcc.Count > 0);

        RuleForEach(x => x.Request.Attachments!)
            .Must(f => f.Length <= 10 * 1024 * 1024)
            .WithMessage("Each attachment must not exceed 10 MB.")
            .When(x => x.Request.Attachments is not null && x.Request.Attachments.Count > 0);
    }
}
