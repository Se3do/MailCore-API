using MailService.Application.DTOs.Attachments;

namespace MailService.Application.DTOs.Emails;

public sealed record ForwardEmailRequest(
    string Body,
    IReadOnlyList<string> To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    IReadOnlyList<CreateAttachmentDto>? Attachments
);
