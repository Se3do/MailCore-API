namespace MailService.Application.DTOs.Label
{
    public record LabelCreateDto(Guid UserId, string Name, string Color, bool IsSystemLabel = false);
}