namespace MailService.Application.DTOs.User
{
    public record UserDto(Guid Id, string Name, string Email, DateTime CreatedAt);
}