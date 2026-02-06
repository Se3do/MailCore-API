namespace MailService.Application.DTOs.Drafts;

public sealed record DraftDto(
    Guid Id,
    string Subject,
    string Body,
    IReadOnlyList<string> To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    Guid? ThreadId,
    DateTimeOffset UpdatedAt
);
