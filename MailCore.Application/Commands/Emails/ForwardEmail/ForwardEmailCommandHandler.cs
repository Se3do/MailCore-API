using MailCore.Application.Emails;
using MailCore.Application.Exceptions;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Emails.ForwardEmail
{
    public class ForwardEmailCommandHandler : IRequestHandler<ForwardEmailCommand>
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IUserRepository _userRepository;
        private readonly EmailComposer _composer;

        public ForwardEmailCommandHandler(
            IEmailRepository emailRepository,
            IUserRepository userRepository,
            EmailComposer composer)
        {
            _emailRepository = emailRepository;
            _userRepository = userRepository;
            _composer = composer;
        }

        public async Task Handle(ForwardEmailCommand command, CancellationToken ct)
        {
            var original = await _emailRepository.GetByIdAsync(command.EmailId, ct)
                ?? throw new NotFoundException("Email not found.");

            if (command.Request.To is null || command.Request.To.Count == 0)
                throw new ValidationException("At least one recipient is required.");

            var sender = await _userRepository.GetByIdAsync(command.UserId, ct)
                ?? throw new NotFoundException("Sender not found.");

            var now = DateTime.UtcNow;

            var thread = await _composer.GetOrCreateThreadAsync(null, now, ct);

            var subject = original.Subject.StartsWith("Fwd:", StringComparison.OrdinalIgnoreCase)
                ? original.Subject
                : $"Fwd: {original.Subject}";
            var email = Email.Create(command.UserId, subject, command.Request.Body, thread.Id);

            await _emailRepository.AddAsync(email, ct);

            await _composer.HandleAttachmentsAsync(email, command.Request.Attachments, ct);
            await _composer.AddRecipientsAsync(email, command.Request.To, command.Request.Cc, command.Request.Bcc, now, ct);
        }
    }
}
