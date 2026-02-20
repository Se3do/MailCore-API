using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Emails;
using MediatR;

namespace MailService.Application.Queries.Email.GetSentPaged
{
    public sealed record GetSentPagedQuery(Guid UserId, CursorPaginationQuery Pagination): IRequest<CursorPagedResult<EmailSummaryDto>>;
}