using MailService.Application.DTOs.Mailbox;
using MailService.Application.Services.Interfaces;
using MailService.Domain.Interfaces;
namespace MailService.Application.Services
{
    public class MailboxService : IMailboxService
    {
        private readonly IMailRecipientRepository _mailRecipientRepository;
        private readonly IUnitOfWork _unitOfWork;
        public MailboxService(IMailRecipientRepository mailRecipientRepository, IUnitOfWork unitOfWork)
        {
            _mailRecipientRepository = mailRecipientRepository;
            _unitOfWork = unitOfWork;
        }

        public Task<bool> DeleteAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<MailboxDetailDto?> GetByMailIdAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<MailboxItemDto>> GetInboxAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<MailboxItemDto>> GetSentAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<MailboxItemDto>> GetSpamAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<MailboxItemDto>> GetStarredAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<MailboxItemDto>> GetTrashAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<MailboxItemDto>> GetUnreadAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MarkReadAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MarkSpamAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MarkUnreadAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RestoreAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> StarAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UnspamAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UnstarAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
