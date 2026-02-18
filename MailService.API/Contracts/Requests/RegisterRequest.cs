namespace MailService.API.Contracts.Requests
{
    public sealed record RegisterRequest(string Name, string Email, string Password);
}
