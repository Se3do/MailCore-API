using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Mailbox;
using MailService.Application.Mappers;
using MailService.Application.Services.Interfaces;
using MailService.Domain.Common;
using MailService.Domain.Interfaces;

namespace MailService.Application.Queries.Mailbox.GetByThreadPaged
{
    public class GetByThreadPagedQueryHandler
    {
        private readonly IMailRecipientRepository _mailRecipientRepository;

        public GetByThreadPagedQueryHandler(IMailRecipientRepository mailRecipientRepository)
        {
            _mailRecipientRepository = mailRecipientRepository;
        }

        public async Task<CursorPagedResult<MailboxItemDto>> Handle(
            GetByThreadPagedQuery query,
            CancellationToken ct)
        {
            var cursor = query.Pagination.ToCursor();
            var pageSize = query.Pagination.PageSize;

            var mails = await _mailRecipientRepository
                .GetByThreadPagedAsync(
                    query.UserId,
                    query.ThreadId,
                    cursor,
                    pageSize,
                    ct);

            return CursorPaginationHelper.Build(
                mails,
                pageSize,
                m => new Cursor(m.ReceivedAt, m.Id),
                m => m.ToMailboxItemDto());
        }
    }
}
