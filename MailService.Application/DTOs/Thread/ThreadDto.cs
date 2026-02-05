namespace MailService.Application.DTOs.Thread
{
    public record ThreadDto(Guid Id, DateTime CreatedAt, DateTime LastMessageAt);
}