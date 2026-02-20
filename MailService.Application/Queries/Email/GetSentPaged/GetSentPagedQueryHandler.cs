using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Emails;
using MailService.Application.DTOs.Mailbox;
using MailService.Application.Mappers;
using MailService.Domain.Common;
using MailService.Domain.Interfaces;

namespace MailService.Application.Queries.Email.GetSentPaged
{
    public class GetSentPagedQueryHandler
    {
        private readonly IEmailRepository _emailRepository;

        public GetSentPagedQueryHandler(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<CursorPagedResult<EmailSummaryDto>> Handle(GetSentPagedQuery query, CancellationToken ct)
        {
            var cursor = query.Pagination.ToCursor();
            var pageSize = query.Pagination.PageSize;

            var mails = await _emailRepository
                .GetSentPagedAsync(query.UserId, cursor, pageSize, ct);

            return CursorPaginationHelper.Build(
                mails,
                pageSize,
                e => new Cursor(e.CreatedAt, e.Id),
                e => e.ToSummaryDto());
        }
    }
}

