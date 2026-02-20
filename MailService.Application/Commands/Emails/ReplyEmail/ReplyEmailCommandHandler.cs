using MailService.Application.Emails;
using MailService.Domain.Entities;
using MailService.Domain.Enums;
using MailService.Domain.Interfaces;

namespace MailService.Application.Commands.Emails.ReplyEmail
{
    public class ReplyEmailCommandHandler
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IUserRepository _userRepository;
        private readonly IThreadRepository _threadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly EmailComposer _composer;

        public ReplyEmailCommandHandler(
            IEmailRepository emailRepository,
            IUserRepository userRepository,
            IThreadRepository threadRepository,
            IUnitOfWork unitOfWork,
            EmailComposer composer)
        {
            _emailRepository = emailRepository;
            _userRepository = userRepository;
            _threadRepository = threadRepository;
            _unitOfWork = unitOfWork;
            _composer = composer;
        }

        public async Task Handle(ReplyEmailCommand command, CancellationToken ct = default)
        {
            var original = await _emailRepository.GetByIdAsync(command.EmailId, ct)
                           ?? throw new KeyNotFoundException("Email not found.");

            var sender = await _userRepository.GetByIdAsync(command.UserId, ct)
                         ?? throw new KeyNotFoundException("Sender not found.");

            var toList = command.Request.To is { Count: > 0 }
                ? command.Request.To
                : await GetDefaultReplyRecipientsAsync(original, ct);

            if (toList.Count == 0)
                throw new ArgumentException("At least one recipient is required.");

            var thread = await _threadRepository.GetByIdAsync(original.ThreadId, ct)
                         ?? throw new KeyNotFoundException("Thread not found.");

            var now = DateTime.UtcNow;
            thread.LastMessageAt = now;

            var email = new Email
            {
                Id = Guid.NewGuid(),
                SenderId = command.UserId,
                Subject = original.Subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase)
                    ? original.Subject
                    : $"Re: {original.Subject}",
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

            await _unitOfWork.SaveChangesAsync(ct);
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
    }
}
