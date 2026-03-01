using MailCore.Domain.Common;
using MailCore.Domain.Entities;

namespace MailCore.Domain.Interfaces
{
    public interface IEmailRepository
    {
        Task AddAsync(Email email, CancellationToken cancellationToken = default);
        Task<Email?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Email>> GetSentPagedAsync(Guid userId, Cursor cursor, int pageSize, CancellationToken cancellationToken = default);
    }
}