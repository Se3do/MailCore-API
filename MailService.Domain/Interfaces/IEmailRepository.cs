using MailService.Domain.Entities;
using System.Threading.Tasks;

namespace MailService.Domain.Interfaces
{
    public interface IEmailRepository
    {
        Task AddAsync(Email email, CancellationToken cancellationToken = default);
        Task<Email?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Email>> GetSentAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}