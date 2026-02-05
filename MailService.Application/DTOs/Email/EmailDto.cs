using MailService.Application.DTOs.Attachment;
using MailService.Application.DTOs.Recipient;

namespace MailService.Application.DTOs.Email
{
    public record EmailDto(
        Guid Id,
        Guid ThreadId,
        Guid SenderId,
        string SenderName,
        string Subject,
        string Body,
        DateTime CreatedAt,
        bool HasAttachments,
        IReadOnlyList<RecipientDto> Recipients,
        IReadOnlyList<AttachmentDto> Attachments);
}