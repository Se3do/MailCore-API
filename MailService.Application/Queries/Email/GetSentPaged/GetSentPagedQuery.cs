using MailService.Application.Common.Pagination;

namespace MailService.Application.Queries.Email.GetSentPaged
{
    public sealed record GetSentPagedQuery(Guid UserId, CursorPaginationQuery Pagination);
}