using MailService.Application.DTOs.Label;

namespace MailService.Application.Interfaces.Services
{
    public interface ILabelService
    {
        Task<Guid> CreateAsync(LabelCreateDto label, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid id, LabelUpdateDto label, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<LabelDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<LabelDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}