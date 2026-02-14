using MailService.Application.DTOs.Attachments;
using Microsoft.AspNetCore.Http;

namespace MailService.Application.DTOs.Emails;

public sealed record ForwardEmailRequest(
    string Body,
    IReadOnlyList<string> To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    IReadOnlyList<IFormFile>? Attachments
);
