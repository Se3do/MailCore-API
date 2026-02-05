using MailService.Application.DTOs.Recipient;

namespace MailService.Application.Interfaces.Services
{
    public interface IMailRecipientService
    {
        Task<RecipientDto?> GetByIdAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RecipientDto>> GetInboxAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RecipientDto>> GetUnreadAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RecipientDto>> GetStarredAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RecipientDto>> GetByLabelAsync(Guid userId, Guid labelId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RecipientDto>> GetDeletedAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(MailRecipientCreateDto recipient, CancellationToken cancellationToken = default);
    }
}