using MailService.Application.Common.Pagination;

namespace MailService.Application.Queries.Mailbox.GetStarredPaged
{
    public sealed record GetStarredPagedQuery(Guid UserId, CursorPaginationQuery Pagination);
}