using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Mailbox;
using MediatR;

namespace MailService.Application.Queries.Mailbox.GetTrashPaged
{
    public sealed record GetTrashPagedQuery(Guid UserId, CursorPaginationQuery Pagination) : IRequest<CursorPagedResult<MailboxItemDto>>;
}