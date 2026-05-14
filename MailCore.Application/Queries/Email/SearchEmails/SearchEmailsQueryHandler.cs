using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Emails;
using MailCore.Application.Mappers;
using MailCore.Domain.Common;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Queries.Email.SearchEmails;

public class SearchEmailsQueryHandler : IRequestHandler<SearchEmailsQuery, CursorPagedResult<EmailSummaryDto>>
{
    private readonly IEmailRepository _emailRepository;

    public SearchEmailsQueryHandler(IEmailRepository emailRepository)
    {
        _emailRepository = emailRepository;
    }

    public async Task<CursorPagedResult<EmailSummaryDto>> Handle(SearchEmailsQuery query, CancellationToken ct)
    {
        var cursor = query.Pagination.ToCursor();
        var pageSize = query.Pagination.PageSize;

        var emails = await _emailRepository.SearchPagedAsync(
            query.UserId, query.Query, cursor, pageSize, ct);

        return CursorPaginationHelper.Build(
            emails,
            pageSize,
            (Domain.Entities.Email e) => new Cursor(e.CreatedAt, e.Id),
            e => e.ToSummaryDto());
    }
}
