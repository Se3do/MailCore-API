using MailService.Application.Commands.Emails.ForwardEmail;
using MailService.Application.Commands.Emails.ReplyEmail;
using MailService.Application.Commands.Emails.SendEmail;
using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Emails;
using MailService.Application.Queries.Email.GetSentById;
using MailService.Application.Queries.Email.GetSentPaged;
using MailService.Application.Services.Interfaces;

namespace MailService.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly SendEmailCommandHandler _send;
        private readonly ReplyEmailCommandHandler _reply;
        private readonly ForwardEmailCommandHandler _forward;
        private readonly GetSentPagedQueryHandler _getSentPaged;
        private readonly GetSentByIdQueryHandler _getSentById;

        public EmailService(
            SendEmailCommandHandler send,
            ReplyEmailCommandHandler reply,
            ForwardEmailCommandHandler forward,
            GetSentPagedQueryHandler getSentPaged,
            GetSentByIdQueryHandler getSentById)
        {
            _send = send;
            _reply = reply;
            _forward = forward;
            _getSentPaged = getSentPaged;
            _getSentById = getSentById;
        }

        public Task SendAsync(Guid userId, SendEmailRequest request, CancellationToken ct)
            => _send.Handle(new SendEmailCommand(userId, request), ct);

        public Task ReplyAsync(Guid userId, Guid emailId, ReplyEmailRequest request, CancellationToken ct)
            => _reply.Handle(new ReplyEmailCommand(userId, emailId, request), ct);

        public Task ForwardAsync(Guid userId, Guid emailId, ForwardEmailRequest request, CancellationToken ct)
            => _forward.Handle(new ForwardEmailCommand(userId, emailId, request), ct);

        public Task<CursorPagedResult<EmailSummaryDto>> GetSentPagedAsync(Guid userId, CursorPaginationQuery query, CancellationToken ct)
            => _getSentPaged.Handle(new GetSentPagedQuery(userId, query), ct);

        public Task<EmailDto?> GetSentByIdAsync(Guid userId, Guid emailId, CancellationToken ct)
            => _getSentById.Handle(new GetSentByIdQuery(userId, emailId), ct);
    }

}
