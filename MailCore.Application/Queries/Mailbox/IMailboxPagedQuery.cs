using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Mailbox;
using MediatR;

namespace MailCore.Application.Queries.Mailbox
{
    public interface IMailboxPagedQuery : IRequest<CursorPagedResult<MailboxItemDto>>
    {
        Guid UserId { get; }
        CursorPaginationQuery Pagination { get; }
    }
}
