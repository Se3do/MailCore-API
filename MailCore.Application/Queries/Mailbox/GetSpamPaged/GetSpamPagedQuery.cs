using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Mailbox;

namespace MailCore.Application.Queries.Mailbox.GetSpamPaged
{
    public sealed record GetSpamPagedQuery(Guid UserId, CursorPaginationQuery Pagination) : IMailboxPagedQuery;
}