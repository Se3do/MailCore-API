using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Mailbox;
using MediatR;

namespace MailService.Application.Queries.Mailbox.GetUnreadPaged
{
    public sealed record GetUnreadPagedQuery(Guid UserId, CursorPaginationQuery Pagination) : IRequest<CursorPagedResult<MailboxItemDto>>;
}