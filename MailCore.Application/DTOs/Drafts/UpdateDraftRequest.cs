namespace MailCore.Application.DTOs.Drafts;

public sealed record UpdateDraftRequest(
    string Subject,
    string Body
);
