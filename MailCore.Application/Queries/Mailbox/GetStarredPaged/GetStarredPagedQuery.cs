using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Mailbox;

namespace MailCore.Application.Queries.Mailbox.GetStarredPaged
{
    public sealed record GetStarredPagedQuery(Guid UserId, CursorPaginationQuery Pagination) : IMailboxPagedQuery;
}