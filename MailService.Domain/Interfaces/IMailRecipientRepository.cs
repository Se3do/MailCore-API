using MailService.Domain.Entities;
using MailService.Domain.Common;

namespace MailService.Domain.Interfaces
{
    public interface IMailRecipientRepository
    {
        // TODO: Add pagination to methods that return lists
        // TODO: Add Filter By Type (To, Cc, Bcc) where applicable
        Task<MailRecipient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<MailRecipient?> GetByUserAndEmailAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MailRecipient>> GetByThreadAsync(Guid userId, Guid threadId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetInboxAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetUnreadAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetStarredAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetSpamAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetByLabelAsync(Guid userId, Guid labelId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetDeletedAsync(Guid userId, CancellationToken cancellationToken = default);

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
