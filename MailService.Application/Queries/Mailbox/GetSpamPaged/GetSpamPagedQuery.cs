using MailService.Application.Common.Pagination;

namespace MailService.Application.Queries.Mailbox.GetSpamPaged
{
    public sealed record GetSpamPagedQuery(Guid UserId, CursorPaginationQuery Pagination);
}