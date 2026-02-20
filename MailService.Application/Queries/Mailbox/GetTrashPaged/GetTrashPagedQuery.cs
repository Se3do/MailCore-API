using MailService.Application.Common.Pagination;

namespace MailService.Application.Queries.Mailbox.GetTrashPaged
{
    public sealed record GetTrashPagedQuery(Guid UserId, CursorPaginationQuery Pagination);
}