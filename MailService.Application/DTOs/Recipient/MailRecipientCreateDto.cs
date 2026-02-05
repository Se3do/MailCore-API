using MailService.Domain.Enums;

namespace MailService.Application.DTOs.Recipient
{
    public record MailRecipientCreateDto(Guid UserId, Guid EmailId, RecipientType Type);
}