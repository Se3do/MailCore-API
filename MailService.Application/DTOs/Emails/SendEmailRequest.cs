using MailService.Application.DTOs.Attachments;
using Microsoft.AspNetCore.Http;
namespace MailService.Application.DTOs.Emails;

public sealed record SendEmailRequest(
    string Subject,
    string Body,
    IReadOnlyList<string> To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    Guid? ThreadId,
    IReadOnlyList<IFormFile>? Attachments
);
