using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Drafts;

namespace MailService.Application.Services.Interfaces;

public interface IDraftService
{
    Task<DraftDto?> GetByIdAsync(Guid userId, Guid draftId, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(Guid userId, CreateDraftRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid userId, Guid draftId, UpdateDraftRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid userId, Guid draftId, CancellationToken cancellationToken = default);
    Task<CursorPagedResult<DraftDto>> GetAllPagedAsync(Guid userId, CursorPaginationQuery query, CancellationToken cancellationToken = default);
}
