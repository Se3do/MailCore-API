using MailService.Application.Common.Pagination;

namespace MailService.Application.Queries.Mailbox.GetInboxPaged
{
    public sealed record GetInboxPagedQuery(Guid UserId, CursorPaginationQuery Pagination);
}