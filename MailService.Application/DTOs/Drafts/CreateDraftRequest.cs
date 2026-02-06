namespace MailService.Application.DTOs.Drafts;

public sealed record CreateDraftRequest(
    string Subject,
    string Body,
    IReadOnlyList<string> To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    Guid? ThreadId
);