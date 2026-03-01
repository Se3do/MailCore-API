namespace MailCore.Application.DTOs.Drafts;

public sealed record CreateDraftRequest(
    string Subject,
    string Body,
    Guid? ThreadId
);