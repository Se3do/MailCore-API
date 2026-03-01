using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Emails;
using MediatR;

namespace MailCore.Application.Queries.Email.GetSentPaged
{
    public sealed record GetSentPagedQuery(Guid UserId, CursorPaginationQuery Pagination): IRequest<CursorPagedResult<EmailSummaryDto>>;
}