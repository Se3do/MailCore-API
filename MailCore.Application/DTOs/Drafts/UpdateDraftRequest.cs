namespace MailCore.Application.DTOs.Drafts;

/// <summary>Request to update an existing draft email.</summary>
public sealed record UpdateDraftRequest(
    string Subject,
    string Body,
    IReadOnlyList<string>? To = null,
    IReadOnlyList<string>? Cc = null,
    IReadOnlyList<string>? Bcc = null
);
