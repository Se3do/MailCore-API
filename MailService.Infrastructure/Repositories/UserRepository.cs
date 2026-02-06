using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using MailService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MailService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MailServiceDbContext _context;

        public UserRepository(MailServiceDbContext context)
        {
            _context = context;
        }
        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Name == userName, cancellationToken);
        }

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email, cancellationToken);
        }

        public Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Name == userName, cancellationToken);
        }
        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            return user;
        }
    }
}
