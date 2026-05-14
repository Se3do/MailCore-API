namespace MailCore.Application.DTOs.Drafts;

/// <summary>Request to create a new draft email.</summary>
public sealed record CreateDraftRequest(
    string Subject,
    string Body,
    Guid? ThreadId,
    IReadOnlyList<string>? To = null,
    IReadOnlyList<string>? Cc = null,
    IReadOnlyList<string>? Bcc = null
);