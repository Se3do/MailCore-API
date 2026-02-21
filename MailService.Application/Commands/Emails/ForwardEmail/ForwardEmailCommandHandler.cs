using MailService.Application.Emails;
using MailService.Domain.Entities;
using MailService.Domain.Enums;
using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Commands.Emails.ForwardEmail
{
    public class ForwardEmailCommandHandler : IRequestHandler<ForwardEmailCommand>
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IUserRepository _userRepository;
        private readonly IThreadRepository _threadRepository;
        private readonly EmailComposer _composer;

        public ForwardEmailCommandHandler(
            IEmailRepository emailRepository,
            IUserRepository userRepository,
            IThreadRepository threadRepository,
            EmailComposer composer)
        {
            _emailRepository = emailRepository;
            _userRepository = userRepository;
            _threadRepository = threadRepository;
            _composer = composer;
        }

        public async Task Handle(ForwardEmailCommand command, CancellationToken ct)
        {
            var original = await _emailRepository.GetByIdAsync(command.EmailId, ct)
                           ?? throw new KeyNotFoundException("Email not found.");

            if (command.Request.To is null || command.Request.To.Count == 0)
                throw new ArgumentException("At least one recipient is required.");

            var sender = await _userRepository.GetByIdAsync(command.UserId, ct)
                         ?? throw new KeyNotFoundException("Sender not found.");

            var now = DateTime.UtcNow;

            var thread = new Domain.Entities.Thread
            {
                Id = Guid.NewGuid(),
                CreatedAt = now,
                LastMessageAt = now
            };

            await _threadRepository.AddAsync(thread, ct);

            var email = new Email
            {
                Id = Guid.NewGuid(),
                SenderId = command.UserId,
                Subject = original.Subject.StartsWith("Fwd:", StringComparison.OrdinalIgnoreCase)
                    ? original.Subject
                    : $"Fwd: {original.Subject}",
                Body = command.Request.Body,
                CreatedAt = now,
                ThreadId = thread.Id
            };

            await _emailRepository.AddAsync(email, ct);

            await _composer.HandleAttachmentsAsync(email, command.Request.Attachments, ct);
            await _composer.AddRecipientsAsync(email, command.Request.To, RecipientType.To, now, ct);

            if (command.Request.Cc?.Any() == true)
                await _composer.AddRecipientsAsync(email, command.Request.Cc, RecipientType.Cc, now, ct);

            if (command.Request.Bcc?.Any() == true)
                await _composer.AddRecipientsAsync(email, command.Request.Bcc, RecipientType.Bcc, now, ct);
        }
    }
}
