using MailCore.Application.DTOs.Attachments;
using Microsoft.AspNetCore.Http;
namespace MailCore.Application.DTOs.Emails;

public sealed record ReplyEmailRequest(
    string Body,
    IReadOnlyList<string>? To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    IReadOnlyList<IFormFile>? Attachments
);
