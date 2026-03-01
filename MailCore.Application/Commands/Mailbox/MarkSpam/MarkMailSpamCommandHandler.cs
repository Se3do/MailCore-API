using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Mailbox.MarkSpam
{
    public class MarkMailSpamCommandHandler: IRequestHandler<MarkMailSpamCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;

        public MarkMailSpamCommandHandler(IMailRecipientRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(MarkMailSpamCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.IsSpam = true;
            return true;
        }
    }
}
