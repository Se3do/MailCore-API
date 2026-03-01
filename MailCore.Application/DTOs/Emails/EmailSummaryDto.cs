namespace MailCore.Application.DTOs.Emails;
public sealed record EmailSummaryDto(
    Guid Id,
    string Subject,
    string Preview,
    string From,
    DateTimeOffset SentAt,
    Guid? ThreadId,
    bool HasAttachments
);
