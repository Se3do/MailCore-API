using MailService.Application.DTOs.Attachments;
using MailService.Application.DTOs.Labels;

namespace MailService.Application.DTOs.Emails;

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
    IReadOnlyList<AttachmentDto> Attachments,
    IReadOnlyList<LabelDto> Labels
);
