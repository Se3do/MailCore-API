using MailService.Application.Common.Pagination;

namespace MailService.Application.Queries.Drafts.GetDraftsPaged
{
    public record GetDraftsPagedQuery(Guid UserId, CursorPaginationQuery Pagination);
}
