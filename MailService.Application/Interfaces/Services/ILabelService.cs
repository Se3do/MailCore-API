using MailService.Application.DTOs.Labels;

namespace MailService.Application.Services.Interfaces;

public interface ILabelService
{
    Task<IReadOnlyList<LabelDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<LabelDto> CreateAsync(Guid userId, CreateLabelRequest request, CancellationToken cancellationToken = default);
    Task<LabelDto> UpdateAsync(Guid userId, Guid labelId, UpdateLabelRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid userId, Guid labelId, CancellationToken cancellationToken = default);
    Task<bool> AssignLabelAsync(Guid userId, Guid mailId, Guid labelId, CancellationToken cancellationToken = default);
    Task<bool> UnassignLabelAsync(Guid userId, Guid mailId, Guid labelId, CancellationToken cancellationToken = default);

}
