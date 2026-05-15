using MailCore.Application.Models;

namespace MailCore.Application.DTOs.Emails;

/// <summary>Request to forward an existing email to new recipients.</summary>
public sealed record ForwardEmailRequest(
    string Body,
    IReadOnlyList<string> To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    IReadOnlyList<FileData>? Attachments
);
