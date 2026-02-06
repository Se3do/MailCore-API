using System;
using System.Threading;
using System.Threading.Tasks;
using MailService.Domain.Entities;

namespace MailService.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    }
}
