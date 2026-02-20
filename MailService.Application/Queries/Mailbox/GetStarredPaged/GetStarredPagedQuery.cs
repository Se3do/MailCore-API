using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Mailbox;
using MediatR;

namespace MailService.Application.Queries.Mailbox.GetStarredPaged
{
    public sealed record GetStarredPagedQuery(Guid UserId, CursorPaginationQuery Pagination) : IRequest<CursorPagedResult<MailboxItemDto>>;
}