using MailCore.Application.Exceptions;
using MailCore.Application.Interfaces.Services;
using MailCore.Application.Models;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Emails
{
    public class EmailComposer
    {
        private readonly IUserRepository _userRepository;
        private readonly IMailRecipientRepository _mailRecipientRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly IThreadRepository _threadRepository;

        public EmailComposer(
            IUserRepository userRepository,
            IMailRecipientRepository mailRecipientRepository,
            IAttachmentService attachmentService,
            IThreadRepository threadRepository)
        {
            _userRepository = userRepository;
            _mailRecipientRepository = mailRecipientRepository;
            _attachmentService = attachmentService;
            _threadRepository = threadRepository;
        }

        public async Task<Domain.Entities.Thread> GetOrCreateThreadAsync(Guid? threadId, DateTime now, CancellationToken ct)
        {
            if (threadId.HasValue)
            {
                var thread = await _threadRepository.GetByIdAsync(threadId.Value, ct)
                    ?? throw new NotFoundException($"Thread {threadId.Value} not found.");

                thread.Touch();
                return thread;
            }

            var newThread = Domain.Entities.Thread.Create(createdAt: now, lastMessageAt: now);

            await _threadRepository.AddAsync(newThread, ct);
            return newThread;
        }

        public async Task AddRecipientsAsync(
            Email email,
            IReadOnlyList<string>? to,
            IReadOnlyList<string>? cc,
            IReadOnlyList<string>? bcc,
            DateTime receivedAt,
            CancellationToken ct)
        {
            if (to is { Count: > 0 })
                await AddRecipientsAsync(email, to, RecipientType.To, receivedAt, ct);

            if (cc is { Count: > 0 })
                await AddRecipientsAsync(email, cc, RecipientType.Cc, receivedAt, ct);

            if (bcc is { Count: > 0 })
                await AddRecipientsAsync(email, bcc, RecipientType.Bcc, receivedAt, ct);
        }

        public async Task AddRecipientsAsync(
            Email email,
            IEnumerable<string> recipients,
            RecipientType type,
            DateTime receivedAt,
            CancellationToken ct)
        {
            foreach (var address in recipients
                         .Where(r => !string.IsNullOrWhiteSpace(r))
                         .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var user = await _userRepository.GetByEmailAsync(address, ct)
                           ?? throw new NotFoundException($"Recipient not found: {address}");

                var mr = MailRecipient.Create(user.Id, email.Id, type, receivedAt);
                await _mailRecipientRepository.AddAsync(mr, ct);
            }
        }

        public async Task HandleAttachmentsAsync(
            Email email,
            IReadOnlyCollection<FileData>? attachments,
            CancellationToken ct)
        {
            if (attachments is not { Count: > 0 })
                return;

            await _attachmentService.AddAsync(email, attachments, ct);
            email.AttachFiles();
        }
    }

}
