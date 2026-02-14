using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using MailService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MailService.Infrastructure.Repositories
{
    internal class AttachmentRepository : IAttachmentRepository
    {
        private readonly MailServiceDbContext _dbContext;

        public AttachmentRepository(MailServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Attachment attachment, CancellationToken cancellationToken = default)
        {
            await _dbContext.Attachments.AddAsync(attachment, cancellationToken);
        }
        public async Task<Attachment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Attachments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
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
