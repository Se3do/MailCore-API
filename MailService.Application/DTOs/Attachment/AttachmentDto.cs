namespace MailService.Application.DTOs.Attachment
{
    public record AttachmentDto(
        Guid Id,
        string FileName,
        string ContentType,
        long FileSize);
}