using MailCore.Application.Exceptions;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Mailbox.MarkSpam
{
    public class MarkMailSpamCommandHandler : IRequestHandler<MarkMailSpamCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;

        public MarkMailSpamCommandHandler(IMailRecipientRepository repo) => _repo = repo;

        public async Task<bool> Handle(MarkMailSpamCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct)
                 ?? throw new NotFoundException($"Mail {cmd.MailId} not found.");

            if (mr.UserId != cmd.UserId)
                 throw new ForbiddenException("You do not have access to this mail.");

            mr.IsSpam = true;
            return true;
        }
    }
}
