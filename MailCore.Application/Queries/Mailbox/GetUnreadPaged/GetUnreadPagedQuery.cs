using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Mailbox;
using MediatR;

namespace MailCore.Application.Queries.Mailbox.GetUnreadPaged
{
    public sealed record GetUnreadPagedQuery(Guid UserId, CursorPaginationQuery Pagination) : IRequest<CursorPagedResult<MailboxItemDto>>;
}