using MailService.Domain.Common;
using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using MailService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MailService.Infrastructure.Repositories
{
    public class DraftRepository : IDraftRepository
    {
        private readonly MailServiceDbContext _context;
        public DraftRepository(MailServiceDbContext context)
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
        public async Task<IReadOnlyList<Draft>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Drafts
                .AsNoTracking()
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.UpdatedAt)
                .ToListAsync(cancellationToken);
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