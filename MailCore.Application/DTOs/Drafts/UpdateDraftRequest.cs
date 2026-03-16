namespace MailCore.Application.DTOs.Drafts;

public sealed record UpdateDraftRequest(
    string Subject,
    string Body,
    IReadOnlyList<string>? To = null,
    IReadOnlyList<string>? Cc = null,
    IReadOnlyList<string>? Bcc = null
);
