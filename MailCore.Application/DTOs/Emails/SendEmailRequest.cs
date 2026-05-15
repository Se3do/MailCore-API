using MailCore.Application.Models;
namespace MailCore.Application.DTOs.Emails;

/// <summary>Request to send a new email message.</summary>
public sealed record SendEmailRequest(
    string Subject,
    string Body,
    IReadOnlyList<string> To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    Guid? ThreadId,
    IReadOnlyList<FileData>? Attachments
);
