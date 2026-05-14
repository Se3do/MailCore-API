namespace MailCore.API.Contracts.Requests
{
    /// <summary>Request to register a new user account.</summary>
    public sealed record RegisterRequest(string Name, string Email, string Password);
}
