namespace MailCore.Domain.Interfaces
{
    using MailCore.Domain.Entities;

    public interface IAttachmentRepository
    {
        Task AddAsync(Attachment attachment, CancellationToken cancellationToken = default);
        Task<Attachment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}