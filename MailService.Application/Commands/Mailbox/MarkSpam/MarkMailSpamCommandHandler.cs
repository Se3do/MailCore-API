using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Commands.Mailbox.MarkSpam
{
    public class MarkMailSpamCommandHandler: IRequestHandler<MarkMailSpamCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;
        private readonly IUnitOfWork _uow;

        public MarkMailSpamCommandHandler(IMailRecipientRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<bool> Handle(MarkMailSpamCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.IsSpam = true;
            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }
}
