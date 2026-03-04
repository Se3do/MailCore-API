using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using MailCore.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MailCore.Infrastructure.Repositories
{
    internal class AttachmentRepository : IAttachmentRepository
    {
        private readonly MailCoreDbContext _dbContext;

        public AttachmentRepository(MailCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Attachment attachment, CancellationToken cancellationToken = default)
        {
            await _dbContext.Attachments.AddAsync(attachment, cancellationToken);
        }
        public async Task<Attachment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Attachments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var attachment = await _dbContext.Attachments.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (attachment != null)
            {
                _dbContext.Attachments.Remove(attachment);
            }
        }
    }
}
