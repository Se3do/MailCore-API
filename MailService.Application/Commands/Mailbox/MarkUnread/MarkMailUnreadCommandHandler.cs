using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Commands.Mailbox.MarkUnread
{
    public class MarkMailUnreadCommandHandler: IRequestHandler<MarkMailUnreadCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;

        public MarkMailUnreadCommandHandler(IMailRecipientRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(MarkMailUnreadCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.IsRead = false;
            return true;
        }
    }
}
