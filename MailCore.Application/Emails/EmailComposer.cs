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

        public EmailComposer(
            IUserRepository userRepository,
            IMailRecipientRepository mailRecipientRepository,
            IAttachmentService attachmentService)
        {
            _userRepository = userRepository;
            _mailRecipientRepository = mailRecipientRepository;
            _attachmentService = attachmentService;
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
