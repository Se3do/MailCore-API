namespace MailCore.API.Contracts.Requests;

public sealed record ReplyEmailFormRequest(
    string Body,
    IReadOnlyList<string>? To,
    IReadOnlyList<string>? Cc,
    IReadOnlyList<string>? Bcc,
    IReadOnlyList<IFormFile>? Attachments
);
