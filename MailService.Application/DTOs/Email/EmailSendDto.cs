using MailService.Application.DTOs.Attachment;

namespace MailService.Application.DTOs.Email
{
    public record EmailSendDto(
        Guid SenderId,
        string Subject,
        string Body,
        IReadOnlyList<EmailAddressDto> To,
        IReadOnlyList<EmailAddressDto>? Cc = null,
        IReadOnlyList<EmailAddressDto>? Bcc = null,
        IReadOnlyList<AttachmentCreateDto>? Attachments = null);
}