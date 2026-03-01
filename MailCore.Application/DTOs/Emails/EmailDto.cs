using MailCore.Application.DTOs.Attachments;
using MailCore.Application.DTOs.Labels;

namespace MailCore.Application.DTOs.Emails;

public sealed record EmailDto(
    Guid Id,
    string Subject,
    string Body,
    string From,
    IReadOnlyList<string> To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    DateTimeOffset SentAt,
    Guid? ThreadId,
    IReadOnlyList<AttachmentDto> Attachments
);
