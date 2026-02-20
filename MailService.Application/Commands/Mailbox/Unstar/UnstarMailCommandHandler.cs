using MailService.Domain.Interfaces;

namespace MailService.Application.Commands.Mailbox.Unstar
{
    public class UnstarMailCommandHandler
    {
        private readonly IMailRecipientRepository _repo;
        private readonly IUnitOfWork _uow;

        public UnstarMailCommandHandler(IMailRecipientRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<bool> Handle(UnstarMailCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.IsStarred = false;
            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }
}
