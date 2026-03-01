using MailCore.Application.DTOs.Auth;

namespace MailCore.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto?> LoginAsync(string email, string password, CancellationToken cancellationToken);

        Task<AuthResultDto> RegisterAsync(string name, string email, string password, CancellationToken cancellationToken);
    }
}
