using MailService.Application.DTOs.Attachment;

namespace MailService.Application.DTOs.Draft
{
    public record DraftDto(
        Guid? Id,
        Guid UserId,
        Guid? ThreadId,
        string Subject,
        string Body,
        DateTime UpdatedAt);
}