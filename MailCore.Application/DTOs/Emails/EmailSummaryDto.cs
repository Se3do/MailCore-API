namespace MailCore.Application.DTOs.Emails;
/// <summary>Lightweight email summary for list views (no body content).</summary>
public sealed record EmailSummaryDto(
    Guid Id,
    string Subject,
    string Preview,
    string From,
    DateTimeOffset SentAt,
    Guid? ThreadId,
    bool HasAttachments
);
