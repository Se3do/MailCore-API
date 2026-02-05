namespace MailService.Application.DTOs.User
{
    public record UserCreateDto(string Name, string Email, string Password);
}