using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Mailbox;
using MediatR;

namespace MailService.Application.Queries.Mailbox.GetByLabelPaged
{
    public record GetByLabelPagedQuery(Guid UserId, Guid LabelId, CursorPaginationQuery Pagination) : IRequest<CursorPagedResult<MailboxItemDto>>;
}
