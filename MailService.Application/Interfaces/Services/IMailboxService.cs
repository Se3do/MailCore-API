using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Mailbox;

namespace MailService.Application.Services.Interfaces;

public interface IMailboxService
{
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
    Task<CursorPagedResult<MailboxItemDto>> GetByThreadPagedAsync(Guid userId, Guid threadId, CursorPaginationQuery query, CancellationToken cancellationToken = default);
    Task<CursorPagedResult<MailboxItemDto>> GetByLabelPagedAsync(Guid userId, Guid label, CursorPaginationQuery query, CancellationToken ct);
}
