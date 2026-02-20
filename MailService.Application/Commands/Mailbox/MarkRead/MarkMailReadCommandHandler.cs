using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Commands.Mailbox.MarkRead
{
    public class MarkMailReadCommandHandler: IRequestHandler<MarkMailReadCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;
        private readonly IUnitOfWork _uow;

        public MarkMailReadCommandHandler(IMailRecipientRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<bool> Handle(MarkMailReadCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.IsRead = true;
            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }
}
