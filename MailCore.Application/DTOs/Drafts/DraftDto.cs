namespace MailCore.Application.DTOs.Drafts;

/// <summary>A saved draft email message.</summary>
public sealed record DraftDto(
    Guid Id,
    string Subject,
    string Body,
    Guid? ThreadId,
    IReadOnlyList<string> To,
    IReadOnlyList<string> Cc,
    IReadOnlyList<string> Bcc,
    DateTimeOffset UpdatedAt
);
