using ThreadEntity = MailCore.Domain.Entities.Thread;

namespace MailCore.Domain.Interfaces
{
    public interface IThreadRepository
    {
        Task AddAsync(ThreadEntity thread, CancellationToken cancellationToken = default);
        Task<ThreadEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
