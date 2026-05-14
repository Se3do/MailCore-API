namespace MailCore.Application.DTOs.Attachments;

/// <summary>Metadata for a file attached to an email.</summary>
public sealed record AttachmentDto(
    Guid Id,
    string FileName,
    string ContentType,
    long Size
);
