using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Drafts;
using MediatR;

namespace MailCore.Application.Queries.Drafts.GetDraftsPaged
{
    public record GetDraftsPagedQuery(Guid UserId, CursorPaginationQuery Pagination) : IRequest<CursorPagedResult<DraftDto>>;
}
