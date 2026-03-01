using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Mailbox;
using MediatR;

namespace MailCore.Application.Queries.Mailbox.GetInboxPaged
{
    public sealed record GetInboxPagedQuery(Guid UserId, CursorPaginationQuery Pagination) : IRequest<CursorPagedResult<MailboxItemDto>>;
}