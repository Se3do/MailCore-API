using MailService.Application.Commands.Mailbox.MarkDeleted;
using MailService.Application.Commands.Mailbox.MarkRead;
using MailService.Application.Commands.Mailbox.MarkSpam;
using MailService.Application.Commands.Mailbox.MarkStarred;
using MailService.Application.Commands.Mailbox.MarkUnread;
using MailService.Application.Commands.Mailbox.Restore;
using MailService.Application.Commands.Mailbox.Unspam;
using MailService.Application.Commands.Mailbox.Unstar;
using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Mailbox;
using MailService.Application.Queries.Mailbox.GetByLabelPaged;
using MailService.Application.Queries.Mailbox.GetByThreadPaged;
using MailService.Application.Queries.Mailbox.GetInboxPaged;
using MailService.Application.Queries.Mailbox.GetMailById;
using MailService.Application.Queries.Mailbox.GetSpamPaged;
using MailService.Application.Queries.Mailbox.GetStarredPaged;
using MailService.Application.Queries.Mailbox.GetTrashPaged;
using MailService.Application.Queries.Mailbox.GetUnreadPaged;
using MailService.Application.Services.Interfaces;

namespace MailService.Application.Services
{
    public class MailboxService : IMailboxService
    {
        // Commands
        private readonly MarkMailReadCommandHandler _markRead;
        private readonly MarkMailUnreadCommandHandler _markUnread;
        private readonly MarkMailSpamCommandHandler _markSpam;
        private readonly UnspamMailCommandHandler _unmarkSpam;
        private readonly MarkMailStarredCommandHandler _star;
        private readonly UnstarMailCommandHandler _unstar;
        private readonly MarkMailDeletedCommandHandler _delete;
        private readonly RestoreMailCommandHandler _restore;

        // Queries
        private readonly GetMailByIdQueryHandler _getById;
        private readonly GetInboxPagedQueryHandler _getInboxPaged;
        private readonly GetUnreadPagedQueryHandler _getUnreadPaged;
        private readonly GetStarredPagedQueryHandler _getStarredPaged;
        private readonly GetSpamPagedQueryHandler _getSpamPaged;
        private readonly GetTrashPagedQueryHandler _getTrashPaged;
        private readonly GetByThreadPagedQueryHandler _getByThreadPaged;
        private readonly GetByLabelPagedQueryHandler _getByLabelPaged;

        public MailboxService(
            // Commands
            MarkMailReadCommandHandler markRead,
            MarkMailUnreadCommandHandler markUnread,
            MarkMailSpamCommandHandler markSpam,
            UnspamMailCommandHandler unmarkSpam,
            MarkMailStarredCommandHandler star,
            UnstarMailCommandHandler unstar,
            MarkMailDeletedCommandHandler delete,
            RestoreMailCommandHandler restore,

            // Queries
            GetMailByIdQueryHandler getById,
            GetInboxPagedQueryHandler getInboxPaged,
            GetUnreadPagedQueryHandler getUnreadPaged,
            GetStarredPagedQueryHandler getStarredPaged,
            GetSpamPagedQueryHandler getSpamPaged,
            GetTrashPagedQueryHandler getTrashPaged,
            GetByThreadPagedQueryHandler getByThreadPaged,
            GetByLabelPagedQueryHandler getByLabelPaged)
        {
            _markRead = markRead;
            _markUnread = markUnread;
            _markSpam = markSpam;
            _unmarkSpam = unmarkSpam;
            _star = star;
            _unstar = unstar;
            _delete = delete;
            _restore = restore;

            _getById = getById;
            _getInboxPaged = getInboxPaged;
            _getUnreadPaged = getUnreadPaged;
            _getStarredPaged = getStarredPaged;
            _getSpamPaged = getSpamPaged;
            _getTrashPaged = getTrashPaged;
            _getByThreadPaged = getByThreadPaged;
            _getByLabelPaged = getByLabelPaged;
        }

        // ===================== COMMANDS =====================

        public Task<bool> MarkReadAsync(Guid userId, Guid mailId, CancellationToken ct)
            => _markRead.Handle(new(userId, mailId), ct);

        public Task<bool> MarkUnreadAsync(Guid userId, Guid mailId, CancellationToken ct)
            => _markUnread.Handle(new(userId, mailId), ct);

        public Task<bool> MarkSpamAsync(Guid userId, Guid mailId, CancellationToken ct)
            => _markSpam.Handle(new(userId, mailId), ct);

        public Task<bool> UnspamAsync(Guid userId, Guid mailId, CancellationToken ct)
            => _unmarkSpam.Handle(new(userId, mailId), ct);

        public Task<bool> StarAsync(Guid userId, Guid mailId, CancellationToken ct)
            => _star.Handle(new(userId, mailId), ct);

        public Task<bool> UnstarAsync(Guid userId, Guid mailId, CancellationToken ct)
            => _unstar.Handle(new(userId, mailId), ct);

        public Task<bool> DeleteAsync(Guid userId, Guid mailId, CancellationToken ct)
            => _delete.Handle(new(userId, mailId), ct);

        public Task<bool> RestoreAsync(Guid userId, Guid mailId, CancellationToken ct)
            => _restore.Handle(new(userId, mailId), ct);

        // ===================== QUERIES =====================

        public Task<MailboxDetailDto?> GetMailByIdAsync(Guid userId, Guid mailId, CancellationToken ct)
            => _getById.Handle(new(userId, mailId), ct);

        public Task<CursorPagedResult<MailboxItemDto>> GetInboxPagedAsync(
            Guid userId, CursorPaginationQuery query, CancellationToken ct)
            => _getInboxPaged.Handle(new(userId, query), ct);

        public Task<CursorPagedResult<MailboxItemDto>> GetUnreadPagedAsync(
            Guid userId, CursorPaginationQuery query, CancellationToken ct)
            => _getUnreadPaged.Handle(new(userId, query), ct);

        public Task<CursorPagedResult<MailboxItemDto>> GetStarredPagedAsync(
            Guid userId, CursorPaginationQuery query, CancellationToken ct)
            => _getStarredPaged.Handle(new(userId, query), ct);

        public Task<CursorPagedResult<MailboxItemDto>> GetSpamPagedAsync(
            Guid userId, CursorPaginationQuery query, CancellationToken ct)
            => _getSpamPaged.Handle(new(userId, query), ct);

        public Task<CursorPagedResult<MailboxItemDto>> GetTrashPagedAsync(
            Guid userId, CursorPaginationQuery query, CancellationToken ct)
            => _getTrashPaged.Handle(new(userId, query), ct);

        public Task<CursorPagedResult<MailboxItemDto>> GetByThreadPagedAsync(
            Guid userId, Guid threadId, CursorPaginationQuery query, CancellationToken ct)
            => _getByThreadPaged.Handle(new(userId, threadId, query), ct);

        public Task<CursorPagedResult<MailboxItemDto>> GetByLabelPagedAsync(
            Guid userId, Guid label, CursorPaginationQuery query, CancellationToken ct)
            => _getByLabelPaged.Handle(new(userId, label, query), ct);
    }
}
