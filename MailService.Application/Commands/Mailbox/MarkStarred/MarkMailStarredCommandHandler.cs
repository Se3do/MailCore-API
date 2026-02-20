using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Commands.Mailbox.MarkStarred
{
    public class MarkMailStarredCommandHandler: IRequestHandler<MarkMailStarredCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;
        private readonly IUnitOfWork _uow;

        public MarkMailStarredCommandHandler(IMailRecipientRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<bool> Handle(MarkMailStarredCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.IsStarred = true;
            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }
}
