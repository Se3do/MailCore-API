using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Emails;
using MailCore.Application.DTOs.Mailbox;
using MailCore.Application.Mappers;
using MailCore.Domain.Common;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Queries.Email.GetSentPaged
{
    public class GetSentPagedQueryHandler: IRequestHandler<GetSentPagedQuery, CursorPagedResult<EmailSummaryDto>>
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

