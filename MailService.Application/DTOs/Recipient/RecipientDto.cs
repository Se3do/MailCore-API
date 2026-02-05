using MailService.Domain.Enums;

namespace MailService.Application.DTOs.Recipient
{
    public record RecipientDto(
        Guid Id,
        Guid UserId,
        string Email,
        string? Name,
        RecipientType Type,
        bool IsRead,
        bool IsStarred,
        bool IsSpam,
        DateTime ReceivedAt,
        DateTime? DeletedAt);
}