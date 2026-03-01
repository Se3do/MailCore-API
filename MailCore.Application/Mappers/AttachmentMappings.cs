using MailCore.Application.DTOs.Attachments;
using MailCore.Domain.Entities;

namespace MailCore.Application.Mappers;

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
