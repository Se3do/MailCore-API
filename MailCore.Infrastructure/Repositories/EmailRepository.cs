using MailCore.Domain.Common;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
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
        // Read-only display — no tracking needed.
        public async Task<Email?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Emails
                .AsNoTracking()
                .AsSplitQuery()
                .Include(e => e.Sender)
                .Include(e => e.Recipients)
                    .ThenInclude(r => r.User)
                .Include(e => e.Attachments)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }
        public async Task<IReadOnlyList<Email>> GetPendingAsync(int batchSize, CancellationToken cancellationToken = default)
        {
            return await _context.Emails
                .AsTracking()
                .Include(e => e.Attachments)
                .Where(e => e.DeliveryStatus == EmailDeliveryStatus.Pending && e.SendAttempts < DomainConstants.MaxSendAttempts)
                .OrderBy(e => e.CreatedAt)
                .Take(batchSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Email>> GetSentPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Emails
                .AsNoTracking()
                .AsSplitQuery()
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
        public async Task<List<Email>> SearchPagedAsync(Guid userId, string query, Cursor cursor, int pageSize, CancellationToken cancellationToken = default)
        {
            var emails = _context.Emails
                .AsNoTracking()
                .AsSplitQuery()
                .Include(e => e.Sender)
                .Include(e => e.Recipients)
                    .ThenInclude(r => r.User)
                .Where(e =>
                    e.SenderId == userId ||
                    e.Recipients.Any(r => r.UserId == userId));

            if (!string.IsNullOrWhiteSpace(query))
            {
                var searchCondition = BuildSearchCondition(query);

                emails = emails.Where(e =>
                    EF.Functions.Contains(e.Subject, searchCondition) ||
                    EF.Functions.Contains(e.Body, searchCondition) ||
                    EF.Functions.Contains(e.Sender.Email, searchCondition) ||
                    e.Recipients.Any(r => EF.Functions.Contains(r.User.Email, searchCondition)));
            }

            return await emails
                .Where(e =>
                    e.CreatedAt < cursor.Timestamp ||
                    (e.CreatedAt == cursor.Timestamp && e.Id.CompareTo(cursor.Id) < 0))
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                .Take(pageSize + 1)
                .ToListAsync(cancellationToken);
        }

        private static string BuildSearchCondition(string query)
        {
            var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return string.Join(" AND ", words.Select(w => $"\"{w.Replace("\"", "\"\"")}*\""));
        }
    }
}
