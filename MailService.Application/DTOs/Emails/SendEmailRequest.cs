using MailService.Application.DTOs.Attachments;
namespace MailService.Application.DTOs.Emails;

public sealed record SendEmailRequest(
    string Subject,
    string Body,
    IReadOnlyList<string> To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    Guid? ThreadId,
    IReadOnlyList<CreateAttachmentDto>? Attachments
);
