using MailService.Application.DTOs.Attachment;
using MailService.Application.DTOs.Common;
using MailService.Application.DTOs.Email;

namespace MailService.Application.Interfaces.Services
{
    public interface IEmailService
    {
        // Retrieval
        Task<EmailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<EmailDto>> GetInboxAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
        Task<PagedResult<EmailDto>> GetSentAsync(Guid userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        // Sending
        Task<SendResultDto> SendAsync(EmailSendDto email, CancellationToken cancellationToken = default);

        // Recipient / per-user operations (often map to MailRecipient)
        Task MarkAsReadAsync(Guid mailRecipientId, bool isRead, CancellationToken cancellationToken = default);
        Task MoveToFolderAsync(Guid mailRecipientId, string folderName, CancellationToken cancellationToken = default); // folderName could be "Inbox","Trash","Archive", etc.
        Task AddLabelToRecipientAsync(Guid mailRecipientId, Guid labelId, CancellationToken cancellationToken = default);
        Task RemoveLabelFromRecipientAsync(Guid mailRecipientId, Guid labelId, CancellationToken cancellationToken = default);

        // Attachments
        Task<AttachmentDto?> GetAttachmentAsync(Guid attachmentId, CancellationToken cancellationToken = default);
        Task DeleteEmailAsync(Guid emailId, CancellationToken cancellationToken = default);
    }
}
