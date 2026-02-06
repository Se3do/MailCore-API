using MailService.Domain.Entities;

namespace MailService.Domain.Interfaces
{
    public interface IMailRecipientRepository
    {
        // TODO: Add pagination to methods that return lists
        // TODO: Add Filter By Type (To, Cc, Bcc) where applicable
        Task<MailRecipient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<MailRecipient?> GetByUserAndEmailAsync(Guid UserId, Guid MailId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetInboxAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetUnreadAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetStarredAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetByLabelAsync(Guid userId, Guid labelId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MailRecipient>> GetDeletedAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(MailRecipient mailRecipient, CancellationToken cancellationToken = default);
    }
}
