using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Commands.Mailbox.MarkRead
{
    public class MarkMailReadCommandHandler: IRequestHandler<MarkMailReadCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;

        public MarkMailReadCommandHandler(IMailRecipientRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(MarkMailReadCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.IsRead = true;
            return true;
        }
    }
}
