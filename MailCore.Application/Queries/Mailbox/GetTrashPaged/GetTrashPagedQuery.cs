using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Mailbox;

namespace MailCore.Application.Queries.Mailbox.GetTrashPaged
{
    public sealed record GetTrashPagedQuery(Guid UserId, CursorPaginationQuery Pagination) : IMailboxPagedQuery;
}