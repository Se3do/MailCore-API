using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Mailbox;
using MailService.Application.Mappers;
using MailService.Domain.Common;
using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Queries.Mailbox.GetInboxPaged
{
    public class GetInboxPagedQueryHandler: IRequestHandler<GetInboxPagedQuery, CursorPagedResult<MailboxItemDto>>
    {
        private readonly IMailRecipientRepository _mailRecipientRepository;

        public GetInboxPagedQueryHandler(IMailRecipientRepository mailRecipientRepository)
        {
            _mailRecipientRepository = mailRecipientRepository;
        }

        public async Task<CursorPagedResult<MailboxItemDto>> Handle(GetInboxPagedQuery query, CancellationToken ct)
        {
            var cursor = query.Pagination.ToCursor();
            var pageSize = query.Pagination.PageSize;

            var mails = await _mailRecipientRepository
                .GetInboxPagedAsync(query.UserId, cursor, pageSize, ct);

            return CursorPaginationHelper.Build(
                mails,
                pageSize,
                m => new Cursor(m.ReceivedAt, m.Id),
                m => m.ToMailboxItemDto());
        }
    }
}
