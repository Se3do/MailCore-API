using MailCore.Application.DTOs.Attachments;
using Microsoft.AspNetCore.Http;
namespace MailCore.Application.DTOs.Emails;

public sealed record SendEmailRequest(
    string Subject,
    string Body,
    IReadOnlyList<string> To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    Guid? ThreadId,
    IReadOnlyList<IFormFile>? Attachments
);
