using MailService.Application.DTOs.Mailbox;
using MailService.Application.Mappers;
using MailService.Domain.Interfaces;

namespace MailService.Application.Queries.Mailbox.GetMailById
{
    public class GetMailByIdQueryHandler
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
