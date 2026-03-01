using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Mailbox;
using MediatR;

namespace MailCore.Application.Queries.Mailbox.GetByLabelPaged
{
    public record GetByLabelPagedQuery(Guid UserId, Guid LabelId, CursorPaginationQuery Pagination) : IRequest<CursorPagedResult<MailboxItemDto>>;
}
