using MailService.Application.Emails;
using MailService.Application.Interfaces.Services;
using MailService.Domain.Entities;
using MailService.Domain.Enums;
using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Commands.Emails.SendEmail
{
    public class SendEmailCommandHandler: IRequestHandler<SendEmailCommand>
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IUserRepository _userRepository;
        private readonly IThreadRepository _threadRepository;
        private readonly EmailComposer _emailComposer;

        public SendEmailCommandHandler(
            IEmailRepository emailRepository,
            IUserRepository userRepository,
            IThreadRepository threadRepository,
            EmailComposer emailComposer)
        {
            _emailRepository = emailRepository;
            _userRepository = userRepository;
            _threadRepository = threadRepository;
            _emailComposer = emailComposer;
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
                throw new KeyNotFoundException("Sender not found.");
            }

            var now = DateTime.UtcNow;
            var thread = await GetOrCreateThreadAsync(request.ThreadId, now, cancellationToken);

            var email = new Email
            {
                Id = Guid.NewGuid(),
                SenderId = userId,
                Subject = request.Subject,
                Body = request.Body,
                CreatedAt = now,
                ThreadId = thread.Id
            };

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
        }

        private async Task<Domain.Entities.Thread> GetOrCreateThreadAsync(Guid? threadId, DateTime now, CancellationToken ct)
        {
            if (threadId.HasValue)
            {
                var thread = await _threadRepository.GetByIdAsync(threadId.Value, ct)
                             ?? throw new KeyNotFoundException("Thread not found.");

                thread.LastMessageAt = now;
                return thread;
            }

            var newThread = new Domain.Entities.Thread
            {
                Id = Guid.NewGuid(),
                CreatedAt = now,
                LastMessageAt = now
            };

            await _threadRepository.AddAsync(newThread, ct);
            return newThread;
        }
    }
}
