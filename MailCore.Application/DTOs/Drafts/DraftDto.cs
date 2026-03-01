namespace MailCore.Application.DTOs.Drafts;

public sealed record DraftDto(
    Guid Id,
    string Subject,
    string Body,
    Guid? ThreadId,
    DateTimeOffset UpdatedAt
);
