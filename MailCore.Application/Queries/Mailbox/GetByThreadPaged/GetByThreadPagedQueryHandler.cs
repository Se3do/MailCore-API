using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Mailbox;
using MailCore.Application.Mappers;
using MailCore.Domain.Common;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Queries.Mailbox.GetByThreadPaged
{
    public class GetByThreadPagedQueryHandler : IRequestHandler<GetByThreadPagedQuery, CursorPagedResult<MailboxItemDto>>
    {
        private readonly IMailRecipientRepository _mailRecipientRepository;

        public GetByThreadPagedQueryHandler(IMailRecipientRepository mailRecipientRepository)
        {
            _mailRecipientRepository = mailRecipientRepository;
        }

        public async Task<CursorPagedResult<MailboxItemDto>> Handle(GetByThreadPagedQuery query, CancellationToken ct)
        {
            var cursor = query.Pagination.ToCursor();
            var pageSize = query.Pagination.PageSize;

            var mails = await _mailRecipientRepository
                .GetByThreadPagedAsync(query.UserId, query.ThreadId, cursor, pageSize, ct);

            return CursorPaginationHelper.Build(
                mails,
                pageSize,
                m => new Cursor(m.ReceivedAt, m.Id),
                m => m.ToMailboxItemDto());
        }
    }
}
