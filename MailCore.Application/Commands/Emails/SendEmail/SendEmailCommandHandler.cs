using MailCore.Application.Emails;
using MailCore.Application.Exceptions;
using MailCore.Application.Interfaces.Services;
using MailCore.Application.Mappers;
using MailCore.Application.Notifications;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Emails.SendEmail
{
    public class SendEmailCommandHandler: IRequestHandler<SendEmailCommand>
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IUserRepository _userRepository;
        private readonly IThreadRepository _threadRepository;
        private readonly EmailComposer _emailComposer;
        private readonly IPublisher _publisher;

        public SendEmailCommandHandler(
            IEmailRepository emailRepository,
            IUserRepository userRepository,
            IThreadRepository threadRepository,
            EmailComposer emailComposer,
            IPublisher publisher)
        {
            _emailRepository = emailRepository;
            _userRepository = userRepository;
            _threadRepository = threadRepository;
            _emailComposer = emailComposer;
            _publisher = publisher;
        }

        public async Task Handle(SendEmailCommand command, CancellationToken cancellationToken = default)
        {
            var request = command.Request;
            var userId = command.UserId;

            if (request.To is null || request.To.Count == 0)
            {
                throw new ArgumentException("At least one recipient is required.", nameof(request.To));
            }

            var sender = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (sender == null)
            {
                throw new NotFoundException("Sender not found.");
            }

            var now = DateTime.UtcNow;
            var thread = await GetOrCreateThreadAsync(request.ThreadId, now, cancellationToken);

            var email = Email.Create(userId, request.Subject, request.Body, thread.Id, createdAt: now);

            await _emailRepository.AddAsync(email, cancellationToken);

            await _emailComposer.HandleAttachmentsAsync(email, request.Attachments, cancellationToken);
            await _emailComposer.AddRecipientsAsync(email, request.To, RecipientType.To, now, cancellationToken);

            if (request.Cc is { Count: > 0 })
            {
                await _emailComposer.AddRecipientsAsync(email, request.Cc, RecipientType.Cc, now, cancellationToken);
            }

            if (request.Bcc is { Count: > 0 })
            {
                await _emailComposer.AddRecipientsAsync(email, request.Bcc, RecipientType.Bcc, now, cancellationToken);
            }

            await _publisher.Publish(new EmailSentNotification(
                sender.Email,
                request.To,
                email.ToSummaryDto()
            ), cancellationToken);
        }

        private async Task<Domain.Entities.Thread> GetOrCreateThreadAsync(Guid? threadId, DateTime now, CancellationToken ct)
        {
            if (threadId.HasValue)
            {
                var thread = await _threadRepository.GetByIdAsync(threadId.Value, ct)
                             ?? throw new NotFoundException("Thread not found.");

                thread.Touch();
                return thread;
            }

            var newThread = Domain.Entities.Thread.Create(createdAt: now, lastMessageAt: now);

            await _threadRepository.AddAsync(newThread, ct);
            return newThread;
        }
    }
}
