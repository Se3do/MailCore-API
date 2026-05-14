namespace MailCore.API.Contracts.Requests
{
    /// <summary>Request to authenticate an existing user.</summary>
    public sealed record LoginRequest(string Email, string Password);
}
