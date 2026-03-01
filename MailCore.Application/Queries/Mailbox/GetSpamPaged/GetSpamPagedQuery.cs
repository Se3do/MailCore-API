using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Mailbox;
using MediatR;

namespace MailCore.Application.Queries.Mailbox.GetSpamPaged
{
    public sealed record GetSpamPagedQuery(Guid UserId, CursorPaginationQuery Pagination) : IRequest<CursorPagedResult<MailboxItemDto>>;
}