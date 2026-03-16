namespace MailCore.Application.DTOs.Drafts;

public sealed record CreateDraftRequest(
    string Subject,
    string Body,
    Guid? ThreadId,
    IReadOnlyList<string>? To = null,
    IReadOnlyList<string>? Cc = null,
    IReadOnlyList<string>? Bcc = null
);