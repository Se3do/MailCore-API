using MailService.Application.DTOs.Mailbox;

namespace MailService.Application.Services.Interfaces;

public interface IMailboxService
{
    Task<IReadOnlyList<MailboxItemDto>> GetInboxAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MailboxItemDto>> GetSentAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MailboxItemDto>> GetUnreadAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MailboxItemDto>> GetStarredAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MailboxItemDto>> GetSpamAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MailboxItemDto>> GetTrashAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<MailboxDetailDto?> GetByMailIdAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);

    Task<bool> MarkReadAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);
    Task<bool> MarkUnreadAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);
    Task<bool> StarAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);
    Task<bool> UnstarAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);
    Task<bool> MarkSpamAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);
    Task<bool> UnspamAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);
    Task<bool> RestoreAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);

    Task<bool> AddLabelAsync(Guid userId, Guid mailRecipientId, Guid labelId, CancellationToken cancellationToken = default);
    Task<bool> RemoveLabelAsync(Guid userId, Guid mailRecipientId, Guid labelId, CancellationToken cancellationToken = default);
}
