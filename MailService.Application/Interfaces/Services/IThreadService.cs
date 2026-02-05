using MailService.Application.DTOs.Thread;

namespace MailService.Application.Interfaces.Services
{
    public interface IThreadService
    {
        Task<ThreadDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ThreadDto>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}