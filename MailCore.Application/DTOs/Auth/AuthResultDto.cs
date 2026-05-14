namespace MailCore.Application.DTOs.Auth
{
    /// <summary>Result returned after successful authentication containing the user ID and JWT token.</summary>
    public sealed record AuthResultDto(Guid UserId, string Token);
}
