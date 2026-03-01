using MailCore.Domain.Common;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using MailCore.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MailCore.Infrastructure.Repositories
{
    public class DraftRepository : IDraftRepository
    {
        private readonly MailCoreDbContext _context;
        public DraftRepository(MailCoreDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Draft draft, CancellationToken cancellationToken = default)
        {
            await _context.Drafts.AddAsync(draft, cancellationToken);
        }
        public async Task<Draft> UpdateAsync(Guid Id, Draft draft, CancellationToken cancellationToken = default)
        {
            var existingDraft = await _context.Drafts.FirstOrDefaultAsync(d => d.Id == Id, cancellationToken);
            if (existingDraft == null)
            {
                throw new KeyNotFoundException($"Draft with Id {Id} not found.");
            }
            existingDraft.Subject = draft.Subject;
            existingDraft.Body = draft.Body;
            existingDraft.UpdatedAt = DateTime.UtcNow;

            return existingDraft;
        }
        public async Task DeleteAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            var draft = await _context.Drafts.FirstOrDefaultAsync(d => d.Id == Id, cancellationToken);
            if (draft != null)
            {
                _context.Drafts.Remove(draft);
            }
        }
        public async Task<Draft?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Drafts
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }
        public async Task<IReadOnlyList<Draft>> GetAllPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Drafts
                .AsNoTracking()
                .Where(d =>
                    d.UserId == userId &&
                    (
                        d.UpdatedAt < cursor.Timestamp ||
                        (d.UpdatedAt == cursor.Timestamp && d.Id.CompareTo(cursor.Id) < 0)
                    )
                )
                .OrderByDescending(d => d.UpdatedAt)
                .ThenByDescending(d => d.Id)
                .Take(pageSize + 1)
                .ToListAsync(cancellationToken);
        }
    }
}