using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Mailbox;
using MailService.Application.Mappers;
using MailService.Domain.Common;
using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Queries.Mailbox.GetStarredPaged
{
    public class GetStarredPagedQueryHandler : IRequestHandler<GetStarredPagedQuery, CursorPagedResult<MailboxItemDto>>
    {
        private readonly IMailRecipientRepository _repo;

        public GetStarredPagedQueryHandler(IMailRecipientRepository repo)
        {
            _repo = repo;
        }

        public async Task<CursorPagedResult<MailboxItemDto>> Handle(GetStarredPagedQuery query, CancellationToken ct)
        {
            var cursor = query.Pagination.ToCursor();
            var pageSize = query.Pagination.PageSize;

            var mails = await _repo.GetStarredPagedAsync(query.UserId, cursor, pageSize, ct);

            return CursorPaginationHelper.Build(
                mails,
                pageSize,
                m => new Cursor(m.ReceivedAt, m.Id),
                m => m.ToMailboxItemDto());
        }
    }
}
