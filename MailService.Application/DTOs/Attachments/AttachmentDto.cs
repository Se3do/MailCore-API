namespace MailService.Application.DTOs.Attachments;

public sealed record AttachmentDto(
    Guid Id,
    string FileName,
    string ContentType,
    long Size
);
