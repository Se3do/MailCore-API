using MailCore.Domain.Common;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using MailCore.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MailCore.Infrastructure.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly MailCoreDbContext _context;
        public EmailRepository(MailCoreDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Email email, CancellationToken cancellationToken = default)
        {
            await _context.Emails.AddAsync(email, cancellationToken);
        }
        public async Task<Email?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Emails
                .AsNoTracking()
                .Include(e => e.Sender)
                .Include(e => e.Recipients)
                    .ThenInclude(r => r.User)
                .Include(e => e.Attachments)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }
        public async Task<List<Email>> GetSentPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Emails
                .AsNoTracking()
                .Include(e => e.Sender)
                .Include(e => e.Recipients)
                    .ThenInclude(r => r.User)
                .Where(e =>
                    e.SenderId == userId &&
                    (
                        e.CreatedAt < cursor.Timestamp ||
                        (e.CreatedAt == cursor.Timestamp && e.Id.CompareTo(cursor.Id) < 0)
                    )
                )
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                .Take(pageSize + 1)
                .ToListAsync(cancellationToken);
        }
    }
}
