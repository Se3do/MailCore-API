using MailCore.Application.Emails;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Emails.ReplyEmail
{
    public class ReplyEmailCommandHandler: IRequestHandler<ReplyEmailCommand>
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IUserRepository _userRepository;
        private readonly IThreadRepository _threadRepository;
        private readonly EmailComposer _composer;

        public ReplyEmailCommandHandler(
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

        public async Task Handle(ReplyEmailCommand command, CancellationToken ct)
        {
            var original = await _emailRepository.GetByIdAsync(command.EmailId, ct)
                ?? throw new Exception("Email not found.");

            var toList = command.Request.To is { Count: > 0 }
                ? command.Request.To
                : await GetDefaultReplyRecipientsAsync(original, ct);

            if (toList.Count == 0)
                throw new Exception("At least one recipient is required.");

            var thread = await _threadRepository.GetByIdAsync(original.ThreadId, ct)
                ?? throw new Exception("Thread not found.");

            var now = DateTime.UtcNow;
            thread.LastMessageAt = now;

            var email = new Email
            {
                Id = Guid.NewGuid(),
                SenderId = command.UserId,
                Subject = BuildReplySubject(original.Subject),
                Body = command.Request.Body,
                CreatedAt = now,
                ThreadId = thread.Id
            };

            await _emailRepository.AddAsync(email, ct);

            await _composer.HandleAttachmentsAsync(email, command.Request.Attachments, ct);
            await _composer.AddRecipientsAsync(email, toList, RecipientType.To, now, ct);

            if (command.Request.Cc?.Any() == true)
                await _composer.AddRecipientsAsync(email, command.Request.Cc, RecipientType.Cc, now, ct);

            if (command.Request.Bcc?.Any() == true)
                await _composer.AddRecipientsAsync(email, command.Request.Bcc, RecipientType.Bcc, now, ct);
        }

        private async Task<IReadOnlyList<string>> GetDefaultReplyRecipientsAsync(
            Email original,
            CancellationToken ct)
        {
            var sender = await _userRepository.GetByIdAsync(original.SenderId, ct);
            return sender?.Email is { Length: > 0 }
                ? new[] { sender.Email }
                : Array.Empty<string>();
        }

        private static string BuildReplySubject(string subject)
            => subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase)
                ? subject
                : $"Re: {subject}";
    }
}