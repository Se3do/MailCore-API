using MailCore.Application.DTOs.Mailbox;
using MailCore.Application.Exceptions;
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
                .GetByIdAsync(query.MailId, ct)
                ?? throw new NotFoundException($"Mail {query.MailId} not found.");

            if (mailRecipient.UserId != query.UserId)
                throw new ForbiddenException("You do not have access to this mail.");

            return mailRecipient.ToMailboxDetailDto();
        }
    }
}
