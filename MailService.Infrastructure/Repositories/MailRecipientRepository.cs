using MailService.Domain.Common;
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
            return await _context.MailRecipients
                .AsTracking()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Recipients)
                        .ThenInclude(r => r.User)
                .Include(mr => mr.Labels)
                    .ThenInclude(l => l.Label)
                .FirstOrDefaultAsync(mr => mr.UserId == UserId && mr.EmailId == MailId, cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetByLabelAsync(Guid userId, Guid labelId, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsNoTracking()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Where(mr => mr.UserId == userId && mr.Labels.Any(l => l.LabelId == labelId))
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetByLabelPagedAsync(Guid userId, Guid labelId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsNoTracking()
                .AsSplitQuery()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Where(mr =>
                    mr.UserId == userId &&
                    mr.Labels.Any(l => l.LabelId == labelId) &&
                    (
                        mr.ReceivedAt < cursor.Timestamp ||
                        (mr.ReceivedAt == cursor.Timestamp && mr.Id.CompareTo(cursor.Id) < 0)
                    ))
                .OrderByDescending(mr => mr.ReceivedAt)
                .ThenByDescending(mr => mr.Id)
                .Take(pageSize + 1)
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetDeletedAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsNoTracking()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Where(mr => mr.UserId == userId && mr.DeletedAt != null)
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetDeletedPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsNoTracking()
                .AsSplitQuery()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Where(mr =>
                    mr.UserId == userId &&
                    mr.DeletedAt != null &&
                    (
                        mr.ReceivedAt < cursor.Timestamp ||
                        (mr.ReceivedAt == cursor.Timestamp && mr.Id.CompareTo(cursor.Id) < 0)
                    ))
                .OrderByDescending(mr => mr.ReceivedAt)
                .ThenByDescending(mr => mr.Id)
                .Take(pageSize + 1)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<MailRecipient>> GetInboxAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsNoTracking()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Recipients)
                        .ThenInclude(r => r.User)
                .Where(mr => mr.UserId == userId)
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetInboxPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsNoTracking()
                .AsSplitQuery()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Where(mr =>
                    mr.UserId == userId &&
                    (
                        mr.ReceivedAt < cursor.Timestamp ||
                        (mr.ReceivedAt == cursor.Timestamp && mr.Id.CompareTo(cursor.Id) < 0)
                    ))
                .OrderByDescending(mr => mr.ReceivedAt)
                .ThenByDescending(mr => mr.Id)
                .Take(pageSize + 1)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<MailRecipient>> GetStarredAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsNoTracking()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Where(mr => mr.UserId == userId && mr.IsStarred)
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetStarredPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsNoTracking()
                .AsSplitQuery()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Where(mr =>
                    mr.UserId == userId &&
                    mr.IsStarred &&
                    (
                        mr.ReceivedAt < cursor.Timestamp ||
                        (mr.ReceivedAt == cursor.Timestamp && mr.Id.CompareTo(cursor.Id) < 0)
                    ))
                .OrderByDescending(mr => mr.ReceivedAt)
                .ThenByDescending(mr => mr.Id)
                .Take(pageSize + 1)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<MailRecipient>> GetUnreadAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsNoTracking()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Where(mr => mr.UserId == userId && !mr.IsRead)
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetUnreadPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsNoTracking()
                .AsSplitQuery()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Where(mr =>
                    mr.UserId == userId &&
                    !mr.IsRead &&
                    (
                        mr.ReceivedAt < cursor.Timestamp ||
                        (mr.ReceivedAt == cursor.Timestamp && mr.Id.CompareTo(cursor.Id) < 0)
                    ))
                .OrderByDescending(mr => mr.ReceivedAt)
                .ThenByDescending(mr => mr.Id)
                .Take(pageSize + 1)
                .ToListAsync(cancellationToken);
        }

        public async Task<MailRecipient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsTracking()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Recipients)
                        .ThenInclude(r => r.User)
                .Include(mr => mr.Labels)
                    .ThenInclude(l => l.Label)
                .FirstOrDefaultAsync(mr => mr.Id == id, cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetByThreadAsync(Guid userId, Guid threadId, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsNoTracking()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Where(mr => mr.UserId == userId && mr.Email.ThreadId == threadId)
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetByThreadPagedAsync(Guid userId, Guid threadId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsNoTracking()
                .AsSplitQuery()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Where(mr =>
                    mr.UserId == userId &&
                    mr.Email.ThreadId == threadId &&
                    (
                        mr.ReceivedAt < cursor.Timestamp ||
                        (mr.ReceivedAt == cursor.Timestamp && mr.Id.CompareTo(cursor.Id) < 0)
                    ))
                .OrderByDescending(mr => mr.ReceivedAt)
                .ThenByDescending(mr => mr.Id)
                .Take(pageSize + 1)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<MailRecipient>> GetSpamAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsNoTracking()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Where(mr => mr.UserId == userId && mr.IsSpam)
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<MailRecipient>> GetSpamPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.MailRecipients
                .AsNoTracking()
                .AsSplitQuery()
                .Include(mr => mr.Email)
                    .ThenInclude(e => e.Sender)
                .Where(mr =>
                    mr.UserId == userId &&
                    mr.IsSpam &&
                    (
                        mr.ReceivedAt < cursor.Timestamp ||
                        (mr.ReceivedAt == cursor.Timestamp && mr.Id.CompareTo(cursor.Id) < 0)
                    ))
                .OrderByDescending(mr => mr.ReceivedAt)
                .ThenByDescending(mr => mr.Id)
                .Take(pageSize + 1)
                .ToListAsync(cancellationToken);
        }

    }
}
