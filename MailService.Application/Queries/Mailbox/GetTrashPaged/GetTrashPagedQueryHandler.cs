using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Mailbox;
using MailService.Application.Mappers;
using MailService.Application.Queries.Mailbox.GetTrashPaged;
using MailService.Domain.Common;
using MailService.Domain.Interfaces;

namespace MailService.Application.Queries.Mailbox.GetTrashPaged
{
    public class GetTrashPagedQueryHandler
    {
        private readonly IMailRecipientRepository _repo;

        public GetTrashPagedQueryHandler(IMailRecipientRepository repo)
        {
            _repo = repo;
        }

        public async Task<CursorPagedResult<MailboxItemDto>> Handle(GetTrashPagedQuery query, CancellationToken ct)
        {
            var cursor = query.Pagination.ToCursor();
            var pageSize = query.Pagination.PageSize;

            var mails = await _repo.GetDeletedPagedAsync(query.UserId, cursor, pageSize, ct);

            return CursorPaginationHelper.Build(
                mails,
                pageSize,
                m => new Cursor(m.ReceivedAt, m.Id),
                m => m.ToMailboxItemDto());
        }
    }
}
