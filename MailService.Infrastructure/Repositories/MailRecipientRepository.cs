using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using MailService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MailService.Infrastructure.Repositories
{
    public class MailRecipientRepository : IMailRecipientRepository
    {
        MailServiceDbContext _context;
        public MailRecipientRepository(MailServiceDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(MailRecipient mailRecipient, CancellationToken cancellationToken = default)
        {
            await _context.MailRecipients.AddAsync(mailRecipient, cancellationToken);
        }
        public async Task<MailRecipient?> GetByUserAndEmailAsync(Guid UserId, Guid MailId, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients.FirstOrDefaultAsync(mr => mr.UserId == UserId && mr.EmailId == MailId, cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetByLabelAsync(Guid userId, Guid labelId, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .Where(mr => mr.UserId == userId && mr.Labels.Any(l => l.LabelId == labelId))
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetDeletedAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .Where(mr => mr.UserId == userId && mr.DeletedAt != null)
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetInboxAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .Where(mr => mr.UserId == userId)
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetStarredAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .Where(mr => mr.UserId == userId && mr.IsStarred)
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetUnreadAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .Where(mr => mr.UserId == userId && !mr.IsRead)
                .ToListAsync(cancellationToken);
        }
        public async Task<MailRecipient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .Include(mr => mr.Labels)
                    .ThenInclude(l => l.Label)
                .FirstOrDefaultAsync(mr => mr.Id == id, cancellationToken);
        }
    }
}
