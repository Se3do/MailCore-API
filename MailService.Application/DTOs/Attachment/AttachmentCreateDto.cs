namespace MailService.Application.DTOs.Attachment
{
    public record AttachmentCreateDto(
        string FileName,
        string ContentType,
        byte[] Content);
}