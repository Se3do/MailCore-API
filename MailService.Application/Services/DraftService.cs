using MailService.Application.Commands.Drafts.CreateDraft;
using MailService.Application.Commands.Drafts.DeleteDraft;
using MailService.Application.Commands.Drafts.UpdateDraft;
using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Drafts;
using MailService.Application.Queries.Drafts.GetDraftById;
using MailService.Application.Queries.Drafts.GetDraftsPaged;
using MailService.Application.Services.Interfaces;

namespace MailService.Application.Services
{
    public class DraftService : IDraftService
    {
        private readonly CreateDraftCommandHandler _create;
        private readonly UpdateDraftCommandHandler _update;
        private readonly DeleteDraftCommandHandler _delete;
        private readonly GetDraftByIdQueryHandler _getById;
        private readonly GetDraftsPagedQueryHandler _getPaged;

        public DraftService(
            CreateDraftCommandHandler create,
            UpdateDraftCommandHandler update,
            DeleteDraftCommandHandler delete,
            GetDraftByIdQueryHandler getById,
            GetDraftsPagedQueryHandler getPaged)
        {
            _create = create;
            _update = update;
            _delete = delete;
            _getById = getById;
            _getPaged = getPaged;
        }

        public Task<CursorPagedResult<DraftDto>> GetAllPagedAsync(Guid userId, CursorPaginationQuery query, CancellationToken ct)
            => _getPaged.Handle(new(userId, query), ct);

        public Task<DraftDto?> GetByIdAsync(Guid userId, Guid id, CancellationToken ct)
            => _getById.Handle(new(userId, id), ct);

        public Task<Guid> CreateAsync(Guid userId, CreateDraftRequest request, CancellationToken ct)
            => _create.Handle(new(userId, request), ct);

        public Task<bool> UpdateAsync(Guid userId, Guid id, UpdateDraftRequest request, CancellationToken ct)
            => _update.Handle(new(userId, id, request), ct);

        public Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken ct)
            => _delete.Handle(new(userId, id), ct);
    }
}
