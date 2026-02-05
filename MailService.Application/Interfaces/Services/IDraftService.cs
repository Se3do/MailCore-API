using MailService.Application.DTOs.Draft;

namespace MailService.Application.Interfaces.Services
{
    public interface IDraftService
    {
        Task<Guid> CreateAsync(DraftDto draft, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid id, DraftDto draft, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<DraftDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DraftDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}