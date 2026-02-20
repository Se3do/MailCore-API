using MailService.Domain.Interfaces;

namespace MailService.Application.Commands.Mailbox.MarkDeleted
{
    public class MarkMailDeletedCommandHandler
    {
        private readonly IMailRecipientRepository _repo;
        private readonly IUnitOfWork _uow;

        public MarkMailDeletedCommandHandler(IMailRecipientRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<bool> Handle(MarkMailDeletedCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.DeletedAt = DateTime.UtcNow;
            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }
}
