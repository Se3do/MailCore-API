using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Drafts;
using MailService.Application.Mappers;
using MailService.Application.Services.Interfaces;
using MailService.Domain.Common;
using MailService.Domain.Interfaces;

namespace MailService.Application.Services
{
    public class DraftService : IDraftService
    {
        private readonly IDraftRepository _draftRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DraftService(IUnitOfWork unitOfWork, IDraftRepository draftRepository)
        {
            _unitOfWork = unitOfWork;
            _draftRepository = draftRepository;
        }

        public async Task<DraftDto> CreateAsync(Guid userId, CreateDraftRequest request, CancellationToken cancellationToken = default)
        {
            var draft = new Domain.Entities.Draft
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Subject = request.Subject,
                Body = request.Body,
                ThreadId = request.ThreadId,
                UpdatedAt = DateTime.UtcNow
            };
            await _draftRepository.AddAsync(draft, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return draft.ToDto();
        }
        public async Task<DraftDto> UpdateAsync(Guid userId, Guid draftId, UpdateDraftRequest request, CancellationToken cancellationToken = default)
        {
            var draft = await _draftRepository.GetByIdAsync(draftId, cancellationToken);
            if (draft == null || draft.UserId != userId)
            {
                throw new KeyNotFoundException("Draft not found.");
            }

            draft.Subject = request.Subject;
            draft.Body = request.Body;
            draft.UpdatedAt = DateTime.UtcNow;

            await _draftRepository.UpdateAsync(draftId, draft, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return draft.ToDto();
        }
        public async Task<bool> DeleteAsync(Guid userId, Guid draftId, CancellationToken cancellationToken = default)
        {
            var draft = await _draftRepository.GetByIdAsync(draftId, cancellationToken);
            if (draft == null || draft.UserId != userId)
            {
                return false;
            }

            await _draftRepository.DeleteAsync(draftId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IReadOnlyList<DraftDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var drafts = await _draftRepository.GetAllAsync(userId, cancellationToken);
            return drafts.Select(draft => draft.ToDto()).ToList();
        }

        public async Task<DraftDto?> GetByIdAsync(Guid userId, Guid draftId, CancellationToken cancellationToken = default)
        {
            var draft = await _draftRepository.GetByIdAsync(draftId, cancellationToken);
            if (draft == null || draft.UserId != userId)
            {
                return null;
            }

            return draft.ToDto();
        }

        public async Task<CursorPagedResult<DraftDto>> GetAllPagedAsync(Guid userId, CursorPaginationQuery query, CancellationToken cancellationToken = default)
        {
            var cursor = query.ToCursor();
            var pageSize = query.PageSize;

            var drafts = await _draftRepository
                .GetAllPagedAsync(userId, cursor, pageSize, cancellationToken);

            return CursorPaginationHelper.Build(
                drafts,
                pageSize,
                d => new Cursor(d.UpdatedAt, d.Id),
                d => d.ToDto()
            );
        }
    }
}
