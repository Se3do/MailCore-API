using MailService.Domain.Common;
using MailService.Domain.Entities;

namespace MailService.Domain.Interfaces
{
    public interface IDraftRepository
    {
        Task AddAsync(Draft draft, CancellationToken cancellationToken = default);
        Task<Draft> UpdateAsync(Guid Id, Draft draft, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<Draft?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Draft>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Draft>> GetAllPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default);
    }
}