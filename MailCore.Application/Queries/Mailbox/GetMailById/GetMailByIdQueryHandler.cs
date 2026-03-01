using MailCore.Application.DTOs.Mailbox;
using MailCore.Application.Mappers;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Queries.Mailbox.GetMailById
{
    public class GetMailByIdQueryHandler : IRequestHandler<GetMailByIdQuery, MailboxDetailDto?>
    {
        private readonly IMailRecipientRepository _mailRecipientRepository;

        public GetMailByIdQueryHandler(IMailRecipientRepository mailRecipientRepository)
        {
            _mailRecipientRepository = mailRecipientRepository;
        }

        public async Task<MailboxDetailDto?> Handle(GetMailByIdQuery query, CancellationToken ct)
        {
            var mailRecipient = await _mailRecipientRepository
                .GetByIdAsync(query.MailId, ct);

            if (mailRecipient is null || mailRecipient.UserId != query.UserId)
                return null;

            return mailRecipient.ToMailboxDetailDto();
        }
    }
}
