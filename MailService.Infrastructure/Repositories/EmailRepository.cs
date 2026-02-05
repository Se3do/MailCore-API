using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using MailService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MailService.Infrastructure.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly MailServiceDbContext _context;
        public EmailRepository(MailServiceDbContext context)
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
                .Include(e => e.Recipients)
                .Include(e => e.Attachments)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }
        public Task<List<Email>> GetSentAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return _context.Emails
                .AsNoTracking()
                .Where(e => e.SenderId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}
