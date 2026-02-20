using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Drafts;
using MediatR;

namespace MailService.Application.Queries.Drafts.GetDraftsPaged
{
    public record GetDraftsPagedQuery(Guid UserId, CursorPaginationQuery Pagination) : IRequest<CursorPagedResult<DraftDto>>;
}
