using MailService.Application.DTOs.Labels;

namespace MailService.Application.Services.Interfaces;

public interface ILabelService
{
    Task<IReadOnlyList<LabelDto>> GetAllAsync(Guid userId, CancellationToken ct = default);
    Task<Guid> CreateAsync(Guid userId, CreateLabelRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid userId, Guid labelId, UpdateLabelRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid userId, Guid labelId, CancellationToken cancellationToken = default);
    Task<bool> AssignLabelAsync(Guid userId, Guid mailId, Guid labelId, CancellationToken cancellationToken = default);
    Task<bool> UnassignLabelAsync(Guid userId, Guid mailId, Guid labelId, CancellationToken cancellationToken = default);

}
