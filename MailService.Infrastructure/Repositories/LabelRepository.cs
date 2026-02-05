using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using MailService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MailService.Infrastructure.Repositories
{
    public class LabelRepository : ILabelRepository
    {
        private readonly MailServiceDbContext _context;
        public LabelRepository(MailServiceDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Label label, CancellationToken cancellationToken = default)
        {
            await _context.Labels.AddAsync(label, cancellationToken);
        }
        public async Task<Label> UpdateAsync(Guid id, Label label, CancellationToken cancellationToken = default)
        {
            var existingLabel = await _context.Labels
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            if (existingLabel is null)
                throw new KeyNotFoundException($"Label with id '{id}' was not found.");

            existingLabel.Name = label.Name;
            existingLabel.Color = label.Color;
            existingLabel.IsSystemLabel = label.IsSystemLabel;

            return existingLabel;
        }
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var label = await _context.Labels.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
            if (label is null)
                throw new KeyNotFoundException($"Label with id '{id}' was not found.");

            _context.Labels.Remove(label);
        }
        public async Task<List<Label>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Labels
                .AsNoTracking()
                .Where(label => label.UserId == userId)
                .ToListAsync(cancellationToken);
        }
        public async Task<Label?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Labels
                .AsNoTracking()
                .FirstOrDefaultAsync(label => label.Id == id, cancellationToken);
        }
    }
}