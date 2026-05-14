using MailCore.Application.DTOs.Attachments;
using Microsoft.AspNetCore.Http;
namespace MailCore.Application.DTOs.Emails;

/// <summary>Request to reply to an existing email thread.</summary>
public sealed record ReplyEmailRequest(
    string Body,
    IReadOnlyList<string>? To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    IReadOnlyList<IFormFile>? Attachments
);
