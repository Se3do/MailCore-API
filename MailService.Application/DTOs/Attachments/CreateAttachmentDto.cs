namespace MailService.Application.DTOs.Attachments;

public sealed record CreateAttachmentDto(
    string FileName,
    string ContentType,
    string Base64Content
);
