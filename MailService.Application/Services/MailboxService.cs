using MailService.Application.DTOs.Mailbox;
using MailService.Application.Mappers;
using MailService.Application.Services.Interfaces;
using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
namespace MailService.Application.Services
{
    public class MailboxService : IMailboxService
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IMailRecipientRepository _mailRecipientRepository;
        private readonly IUnitOfWork _unitOfWork;
        public MailboxService(IMailRecipientRepository mailRecipientRepository, IUnitOfWork unitOfWork, IEmailRepository emailRepository)
        {
            _mailRecipientRepository = mailRecipientRepository;
            _unitOfWork = unitOfWork;
            _emailRepository = emailRepository;
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            var mailRecipient = await _mailRecipientRepository.GetByIdAsync(mailId, cancellationToken);
            if (mailRecipient is null || mailRecipient.UserId != userId)
            {
                return false;
            }

            mailRecipient.DeletedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<MailboxDetailDto?> GetMailByIdAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            var mailRecipient = await _mailRecipientRepository.GetByIdAsync(mailId, cancellationToken);
            if (mailRecipient is null || mailRecipient.UserId != userId)
            {
                return null;
            }

            return mailRecipient.ToMailboxDetailDto();
        }

        public async Task<IReadOnlyList<MailboxItemDto>> GetInboxAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var emails = await _mailRecipientRepository.GetInboxAsync(userId, cancellationToken);
            return emails.Select(e => e.ToMailboxItemDto()).ToList();
        }

        public async Task<IReadOnlyList<MailboxItemDto>> GetSentAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var emails = await _emailRepository.GetSentAsync(userId, cancellationToken);
            return emails.Select(ToSentMailboxItem).ToList();
        }

        public async Task<IReadOnlyList<MailboxItemDto>> GetSpamAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var emails = await _mailRecipientRepository.GetSpamAsync(userId, cancellationToken);
            return emails.Select(e => e.ToMailboxItemDto()).ToList();
        }

        public async Task<IReadOnlyList<MailboxItemDto>> GetStarredAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var emails = await _mailRecipientRepository.GetStarredAsync(userId, cancellationToken);
            return emails.Select(e => e.ToMailboxItemDto()).ToList();
        }

        public async Task<IReadOnlyList<MailboxItemDto>> GetTrashAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var emails = await _mailRecipientRepository.GetDeletedAsync(userId, cancellationToken);
            return emails.Select(e => e.ToMailboxItemDto()).ToList();
        }

        public async Task<IReadOnlyList<MailboxItemDto>> GetUnreadAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var emails = await _mailRecipientRepository.GetUnreadAsync(userId, cancellationToken);
            return emails.Select(e => e.ToMailboxItemDto()).ToList();
        }

        public async Task<bool> MarkReadAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            var mailRecipient = await _mailRecipientRepository.GetByIdAsync(mailId, cancellationToken);
            if (mailRecipient is null || mailRecipient.UserId != userId)
            {
                return false;
            }

            mailRecipient.IsRead = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> MarkSpamAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            var mailRecipient = await _mailRecipientRepository.GetByIdAsync(mailId, cancellationToken);
            if (mailRecipient is null || mailRecipient.UserId != userId)
            {
                return false;
            }

            mailRecipient.IsSpam = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> MarkUnreadAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            var mailRecipient = await _mailRecipientRepository.GetByIdAsync(mailId, cancellationToken);
            if (mailRecipient is null || mailRecipient.UserId != userId)
            {
                return false;
            }

            mailRecipient.IsRead = false;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> RestoreAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            var mailRecipient = await _mailRecipientRepository.GetByIdAsync(mailId, cancellationToken);
            if (mailRecipient is null || mailRecipient.UserId != userId)
            {
                return false;
            }

            mailRecipient.DeletedAt = null;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> StarAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            var mailRecipient = await _mailRecipientRepository.GetByIdAsync(mailId, cancellationToken);
            if (mailRecipient is null || mailRecipient.UserId != userId)
            {
                return false;
            }

            mailRecipient.IsStarred = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> UnspamAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            var mailRecipient = await _mailRecipientRepository.GetByIdAsync(mailId, cancellationToken);
            if (mailRecipient is null || mailRecipient.UserId != userId)
            {
                return false;
            }

            mailRecipient.IsSpam = false;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> UnstarAsync(Guid userId, Guid mailId, CancellationToken cancellationToken = default)
        {
            var mailRecipient = await _mailRecipientRepository.GetByIdAsync(mailId, cancellationToken);
            if (mailRecipient is null || mailRecipient.UserId != userId)
            {
                return false;
            }

            mailRecipient.IsStarred = false;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IReadOnlyList<MailboxItemDto>> GetByThreadAsync(Guid userId, Guid threadId, CancellationToken cancellationToken = default)
        {
            var mails = await _mailRecipientRepository.GetByThreadAsync(userId, threadId, cancellationToken);
            return mails.Select(m => m.ToMailboxItemDto()).ToList();
        }

        private static MailboxItemDto ToSentMailboxItem(Email email)
        {
            var to = email.Recipients
                .Where(r => r.Type == Domain.Enums.RecipientType.To)
                .Select(r => r.User.Email)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();

            return new MailboxItemDto(
                email.Id,
                email.Id,
                email.Sender.Email,
                to,
                email.Subject,
                CreatePreview(email.Body),
                new DateTimeOffset(email.CreatedAt),
                true,
                false,
                false,
                false,
                email.ThreadId);
        }

        private static string CreatePreview(string body, int maxLength = 100)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return string.Empty;
            }

            return body.Length <= maxLength
                ? body
                : body[..maxLength];
        }
    }
}
