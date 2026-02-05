using MailService.Application.DTOs.User;

namespace MailService.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(UserCreateDto user, CancellationToken cancellationToken = default);
        Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<UserDto?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    }
}
