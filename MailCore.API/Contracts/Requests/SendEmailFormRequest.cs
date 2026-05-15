namespace MailCore.API.Contracts.Requests;

public sealed record SendEmailFormRequest(
    string Subject,
    string Body,
    IReadOnlyList<string> To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    Guid? ThreadId,
    IReadOnlyList<IFormFile>? Attachments
);
