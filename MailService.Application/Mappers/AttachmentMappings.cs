using MailService.Application.DTOs.Attachments;
using MailService.Domain.Entities;

namespace MailService.Application.Mappers;

public static class AttachmentMappings
{
    public static AttachmentDto ToDto(this Attachment attachment)
    {
        return new AttachmentDto(
            attachment.Id,
            attachment.FileName,
            attachment.ContentType,
            attachment.FileSize);
    }
}
