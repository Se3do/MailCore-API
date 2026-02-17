using MailService.Application.Common.Pagination;
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
    Task<IReadOnlyList<MailboxItemDto>> GetByThreadAsync(Guid userId, Guid threadId, CancellationToken cancellationToken = default);

    Task<MailboxDetailDto?> GetMailByIdAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);

    Task<bool> MarkReadAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);
    Task<bool> MarkUnreadAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);
    Task<bool> StarAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);
    Task<bool> UnstarAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);
    Task<bool> MarkSpamAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);
    Task<bool> UnspamAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);
    Task<bool> RestoreAsync(Guid userId, Guid mailRecipientId, CancellationToken cancellationToken = default);

    Task<CursorPagedResult<MailboxItemDto>> GetInboxPagedAsync(Guid userId, CursorPaginationQuery query, CancellationToken cancellationToken = default);
    Task<CursorPagedResult<MailboxItemDto>> GetUnreadPagedAsync(Guid userId, CursorPaginationQuery query, CancellationToken cancellationToken = default);
    Task<CursorPagedResult<MailboxItemDto>> GetStarredPagedAsync(Guid userId, CursorPaginationQuery query, CancellationToken cancellationToken = default);
    Task<CursorPagedResult<MailboxItemDto>> GetSpamPagedAsync(Guid userId, CursorPaginationQuery query, CancellationToken cancellationToken = default);
    Task<CursorPagedResult<MailboxItemDto>> GetTrashPagedAsync(Guid userId, CursorPaginationQuery query, CancellationToken cancellationToken = default);
    Task<CursorPagedResult<MailboxItemDto>> GetSentPagedAsync(Guid userId, CursorPaginationQuery query, CancellationToken cancellationToken = default);
    Task<CursorPagedResult<MailboxItemDto>> GetByThreadPagedAsync(Guid userId, Guid threadId, CursorPaginationQuery query, CancellationToken cancellationToken = default);
}
