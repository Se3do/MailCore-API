using MailService.Domain.Interfaces;

namespace MailService.Application.Commands.Mailbox.MarkUnread
{
    public class MarkMailUnreadCommandHandler
    {
        private readonly IMailRecipientRepository _repo;
        private readonly IUnitOfWork _uow;

        public MarkMailUnreadCommandHandler(IMailRecipientRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<bool> Handle(MarkMailUnreadCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.IsRead = false;
            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }
}
