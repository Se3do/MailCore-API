using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Mailbox.MarkStarred
{
    public class MarkMailStarredCommandHandler: IRequestHandler<MarkMailStarredCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;

        public MarkMailStarredCommandHandler(IMailRecipientRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(MarkMailStarredCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.IsStarred = true;
            return true;
        }
    }
}
