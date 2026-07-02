using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Mailbox;

namespace MailCore.Application.Queries.Mailbox.GetUnreadPaged
{
    public sealed record GetUnreadPagedQuery(Guid UserId, CursorPaginationQuery Pagination) : IMailboxPagedQuery;
}