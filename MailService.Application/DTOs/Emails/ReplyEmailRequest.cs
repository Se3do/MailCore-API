using MailService.Application.DTOs.Attachments;
namespace MailService.Application.DTOs.Emails;

public sealed record ReplyEmailRequest(
    string Body,
    IReadOnlyList<string>? To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    IReadOnlyList<CreateAttachmentDto>? Attachments
);
