namespace MailCore.Application.DTOs.Drafts;

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
