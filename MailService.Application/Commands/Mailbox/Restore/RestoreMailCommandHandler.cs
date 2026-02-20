using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Commands.Mailbox.Restore
{
    public class RestoreMailCommandHandler: IRequestHandler<RestoreMailCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;
        private readonly IUnitOfWork _uow;

        public RestoreMailCommandHandler(IMailRecipientRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<bool> Handle(RestoreMailCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.DeletedAt = null;
            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }
}
