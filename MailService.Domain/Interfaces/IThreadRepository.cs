using ThreadEntity = MailService.Domain.Entities.Thread;

namespace MailService.Domain.Interfaces
{
    public interface IThreadRepository
    {
        Task AddAsync(ThreadEntity thread, CancellationToken cancellationToken = default);
        Task<ThreadEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
