namespace MailCore.API.Contracts.Requests;

public sealed record ForwardEmailFormRequest(
    string Body,
    IReadOnlyList<string> To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    IReadOnlyList<IFormFile>? Attachments
);
