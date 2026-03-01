using MailCore.Domain.Interfaces;
using MailCore.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using ThreadEntity = MailCore.Domain.Entities.Thread;

namespace MailCore.Infrastructure.Repositories
{
    public class ThreadRepository : IThreadRepository
    {
        private readonly MailCoreDbContext _context;

        public ThreadRepository(MailCoreDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ThreadEntity thread, CancellationToken cancellationToken = default)
        {
            await _context.Threads.AddAsync(thread, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task<ThreadEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Threads.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }
    }
}
