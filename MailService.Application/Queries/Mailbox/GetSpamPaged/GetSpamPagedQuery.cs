using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Mailbox;
using MediatR;

namespace MailService.Application.Queries.Mailbox.GetSpamPaged
{
    public sealed record GetSpamPagedQuery(Guid UserId, CursorPaginationQuery Pagination) : IRequest<CursorPagedResult<MailboxItemDto>>;
}