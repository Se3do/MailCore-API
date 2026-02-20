using MailService.Application.Common.Pagination;

namespace MailService.Application.Queries.Mailbox.GetByThreadPaged
{
    public sealed record GetByThreadPagedQuery(Guid UserId, Guid ThreadId, CursorPaginationQuery Pagination);
}