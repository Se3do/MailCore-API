using MailService.Domain.Interfaces;

namespace MailService.Application.Commands.Mailbox.Unspam
{
    public class UnspamMailCommandHandler
    {
        private readonly IMailRecipientRepository _repo;
        private readonly IUnitOfWork _uow;

        public UnspamMailCommandHandler(IMailRecipientRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<bool> Handle(UnspamMailCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.IsSpam = false;
            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }
}
