using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Drafts;
using MailCore.Application.Mappers;
using MailCore.Domain.Common;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Queries.Drafts.GetDraftsPaged
{
    public class GetDraftsPagedQueryHandler : IRequestHandler<GetDraftsPagedQuery, CursorPagedResult<DraftDto>>
    {
        private readonly IDraftRepository _draftRepository;

        public GetDraftsPagedQueryHandler(IDraftRepository draftRepository)
        {
            _draftRepository = draftRepository;
        }

        public async Task<CursorPagedResult<DraftDto>> Handle(GetDraftsPagedQuery query, CancellationToken ct)
        {
            var cursor = query.Pagination.ToCursor();
            var pageSize = query.Pagination.PageSize;
            var pagedDrafts = await _draftRepository.GetAllPagedAsync(query.UserId, cursor, pageSize, ct);

            return CursorPaginationHelper.Build(
                pagedDrafts,
                pageSize,
                d => new Cursor(d.UpdatedAt, d.Id),
                d => d.ToDto()
            );
        }
    }
}
