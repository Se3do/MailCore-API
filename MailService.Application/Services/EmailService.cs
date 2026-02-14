using MailService.Application.DTOs.Emails;
using MailService.Application.Interfaces.Services;
using MailService.Application.Mappers;
using MailService.Application.Services.Interfaces;
using MailService.Domain.Entities;
using MailService.Domain.Enums;
using MailService.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MailService.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMailRecipientRepository _mailRecipientRepository;
        private readonly IThreadRepository _threadRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly IUnitOfWork _unitOfWork;

        public EmailService(IEmailRepository emailRepository, IUnitOfWork unitOfWork, IUserRepository userRepository, IMailRecipientRepository mailRecipientRepository, IThreadRepository threadRepository, IAttachmentService attachmentService)
        {
            _emailRepository = emailRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _mailRecipientRepository = mailRecipientRepository;
            _threadRepository = threadRepository;
            _attachmentService = attachmentService;
        }

        public async Task<EmailDto> SendAsync(Guid userId, SendEmailRequest request, CancellationToken cancellationToken = default)
        {
            if (request.To is null || request.To.Count == 0)
            {
                throw new ArgumentException("At least one recipient is required.", nameof(request.To));
            }

            var sender = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (sender == null)
            {
                throw new KeyNotFoundException("Sender not found.");
            }

            var now = DateTime.UtcNow;
            Domain.Entities.Thread? thread = request.ThreadId.HasValue
                ? await _threadRepository.GetByIdAsync(request.ThreadId.Value, cancellationToken)
                : new Domain.Entities.Thread
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = now,
                    LastMessageAt = now
                };

            if (thread is null)
            {
                throw new KeyNotFoundException("Thread not found.");
            }

            thread.LastMessageAt = now;

            var email = new Email
            {
                Id = Guid.NewGuid(),
                SenderId = userId,
                Sender = sender,
                Subject = request.Subject,
                Body = request.Body,
                CreatedAt = now,
                ThreadId = thread.Id,
                Thread = request.ThreadId.HasValue ? null : thread,
            };

            await _emailRepository.AddAsync(email, cancellationToken);

            await HandleAttachmentsAsync(email, request.Attachments, cancellationToken);

            await AddRecipientsAsync(email, request.To, RecipientType.To, now, cancellationToken);
            if (request.Cc is { Count: > 0 })
            {
                await AddRecipientsAsync(email, request.Cc, RecipientType.Cc, now, cancellationToken);
            }
            if (request.Bcc is { Count: > 0 })
            {
                await AddRecipientsAsync(email, request.Bcc, RecipientType.Bcc, now, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return email.ToDto();
        }

        public async Task<EmailDto> ReplyAsync(Guid userId, Guid emailId, ReplyEmailRequest request, CancellationToken cancellationToken = default)
        {
            var original = await _emailRepository.GetByIdAsync(emailId, cancellationToken);
            if (original == null)
            {
                throw new KeyNotFoundException("Email not found.");
            }

            var sender = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (sender == null)
            {
                throw new KeyNotFoundException("Sender not found.");
            }

            var toList = request.To is { Count: > 0 }
                ? request.To
                : await GetDefaultReplyRecipientsAsync(original, cancellationToken);

            if (toList.Count == 0)
            {
                throw new ArgumentException("At least one recipient is required.", nameof(request.To));
            }

            var now = DateTime.UtcNow;
            Domain.Entities.Thread? thread = await _threadRepository.GetByIdAsync(original.ThreadId, cancellationToken);
            if (thread is null)
            {
                throw new KeyNotFoundException("Thread not found.");
            }

            thread.LastMessageAt = now;

            var email = new Email
            {
                Id = Guid.NewGuid(),
                SenderId = userId,
                Sender = sender,
                Subject = original.Subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase)
                    ? original.Subject
                    : $"Re: {original.Subject}",
                Body = request.Body,
                CreatedAt = now,
                ThreadId = thread.Id,
            };

            await _emailRepository.AddAsync(email, cancellationToken);

            await HandleAttachmentsAsync(email, request.Attachments, cancellationToken);

            await AddRecipientsAsync(email, toList, RecipientType.To, now, cancellationToken);
            if (request.Cc is { Count: > 0 })
            {
                await AddRecipientsAsync(email, request.Cc, RecipientType.Cc, now, cancellationToken);
            }
            if (request.Bcc is { Count: > 0 })
            {
                await AddRecipientsAsync(email, request.Bcc, RecipientType.Bcc, now, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return email.ToDto();
        }

        public async Task<EmailDto> ForwardAsync(Guid userId, Guid emailId, ForwardEmailRequest request, CancellationToken cancellationToken = default)
        {
            var original = await _emailRepository.GetByIdAsync(emailId, cancellationToken);
            if (original == null)
            {
                throw new KeyNotFoundException("Email not found.");
            }

            if (request.To is null || request.To.Count == 0)
            {
                throw new ArgumentException("At least one recipient is required.", nameof(request.To));
            }

            var sender = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (sender == null)
            {
                throw new KeyNotFoundException("Sender not found.");
            }

            var now = DateTime.UtcNow;
            var thread = new Domain.Entities.Thread
            {
                Id = Guid.NewGuid(),
                CreatedAt = now,
                LastMessageAt = now
            };

            var email = new Email
            {
                Id = Guid.NewGuid(),
                SenderId = userId,
                Sender = sender,
                Subject = original.Subject.StartsWith("Fwd:", StringComparison.OrdinalIgnoreCase)
                    ? original.Subject
                    : $"Fwd: {original.Subject}",
                Body = request.Body,
                CreatedAt = now,
                ThreadId = thread.Id,
                Thread = thread,
            };

            await _emailRepository.AddAsync(email, cancellationToken);

            await HandleAttachmentsAsync(email, request.Attachments, cancellationToken);

            await AddRecipientsAsync(email, request.To, RecipientType.To, now, cancellationToken);
            if (request.Cc is { Count: > 0 })
            {
                await AddRecipientsAsync(email, request.Cc, RecipientType.Cc, now, cancellationToken);
            }
            if (request.Bcc is { Count: > 0 })
            {
                await AddRecipientsAsync(email, request.Bcc, RecipientType.Bcc, now, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return email.ToDto();
        }

        private async Task<IReadOnlyList<string>> GetDefaultReplyRecipientsAsync(Email original, CancellationToken cancellationToken)
        {
            var originalSender = await _userRepository.GetByIdAsync(original.SenderId, cancellationToken);
            if (originalSender == null)
            {
                return Array.Empty<string>();
            }

            return new[] { originalSender.Email };
        }
        private async Task AddRecipientsAsync(Email email, IEnumerable<string> recipients, RecipientType type, DateTime receivedAt, CancellationToken cancellationToken)
        {
            foreach (var address in recipients.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var user = await _userRepository.GetByEmailAsync(address, cancellationToken);
                if (user == null)
                {
                    throw new KeyNotFoundException($"Recipient not found: {address}");
                }
                await _mailRecipientRepository.AddAsync(new MailRecipient
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    User = user,
                    EmailId = email.Id,
                    Type = type,
                    IsRead = false,
                    IsSpam = false,
                    IsStarred = false,
                    ReceivedAt = receivedAt
                });
            }
        }

        private async Task HandleAttachmentsAsync(Email email, IReadOnlyCollection<IFormFile>? attachments, CancellationToken cancellationToken)
        {
            if (attachments is not { Count: > 0 })
                return;

            await _attachmentService.AddAsync(email, attachments, cancellationToken);
            email.HasAttachments = true;
        }

    }
}
