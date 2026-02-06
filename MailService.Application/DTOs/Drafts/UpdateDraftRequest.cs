namespace MailService.Application.DTOs.Drafts;

public sealed record UpdateDraftRequest(
    string Subject,
    string Body,
    IReadOnlyList<string> To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc
);
