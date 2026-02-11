using MailService.Domain.Interfaces;
using MailService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using ThreadEntity = MailService.Domain.Entities.Thread;

namespace MailService.Infrastructure.Repositories
{
    public class ThreadRepository : IThreadRepository
    {
        private readonly MailServiceDbContext _context;

        public ThreadRepository(MailServiceDbContext context)
        {
            _context = context;
        }

        public Task<ThreadEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Threads.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }
    }
}
