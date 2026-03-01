namespace MailCore.Application.DTOs.Auth
{
    public sealed record AuthResultDto(Guid UserId, string Token);
}
