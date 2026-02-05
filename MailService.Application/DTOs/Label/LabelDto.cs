namespace MailService.Application.DTOs.Label
{
    public record LabelDto(Guid Id, Guid UserId, string Name, string Color, bool IsSystemLabel);
}