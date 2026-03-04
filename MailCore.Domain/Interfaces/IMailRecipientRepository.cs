using MailCore.Domain.Entities;
using MailCore.Domain.Common;

namespace MailCore.Domain.Interfaces
{
    public interface IMailRecipientRepository
    {
        Task<MailRecipient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetInboxPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetUnreadPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetStarredPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetSpamPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetDeletedPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetByLabelPagedAsync(Guid userId, Guid labelId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetByThreadPagedAsync(Guid userId, Guid threadId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default);

        Task AddAsync(MailRecipient mailRecipient, CancellationToken cancellationToken = default);
    }
}
