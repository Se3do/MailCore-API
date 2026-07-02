using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Mailbox;
using MailCore.Application.Mappers;
using MailCore.Domain.Common;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Queries.Mailbox
{
    public abstract class BaseMailboxPagedQueryHandler<TQuery> : IRequestHandler<TQuery, CursorPagedResult<MailboxItemDto>>
        where TQuery : IMailboxPagedQuery
    {
        private readonly IMailRecipientRepository _repo;

        protected BaseMailboxPagedQueryHandler(IMailRecipientRepository repo) => _repo = repo;

        protected abstract Task<IReadOnlyList<MailRecipient>> FetchAsync(IMailRecipientRepository repo, TQuery query, CancellationToken ct);

        public async Task<CursorPagedResult<MailboxItemDto>> Handle(TQuery query, CancellationToken ct)
        {
            var cursor = query.Pagination.ToCursor();
            var pageSize = query.Pagination.PageSize;

            var mails = await FetchAsync(_repo, query, ct);

            return CursorPaginationHelper.Build(
                mails,
                pageSize,
                m => new Cursor(m.ReceivedAt, m.Id),
                m => m.ToMailboxItemDto());
        }
    }
}
