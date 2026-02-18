using MailService.Application.DTOs.Auth;

namespace MailService.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto?> LoginAsync(string email, string password, CancellationToken cancellationToken);

        Task<AuthResultDto> RegisterAsync(string name, string email, string password, CancellationToken cancellationToken);
    }
}
