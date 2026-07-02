using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Mailbox;

namespace MailCore.Application.Queries.Mailbox.GetByThreadPaged
{
    public sealed record GetByThreadPagedQuery(Guid UserId, Guid ThreadId, CursorPaginationQuery Pagination) : IMailboxPagedQuery;
}