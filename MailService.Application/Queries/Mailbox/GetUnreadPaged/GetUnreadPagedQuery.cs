using MailService.Application.Common.Pagination;

namespace MailService.Application.Queries.Mailbox.GetUnreadPaged
{
    public sealed record GetUnreadPagedQuery(Guid UserId, CursorPaginationQuery Pagination);
}