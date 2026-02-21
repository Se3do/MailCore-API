using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Commands.Mailbox.Unspam
{
    public class UnspamMailCommandHandler: IRequestHandler<UnspamMailCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;

        public UnspamMailCommandHandler(IMailRecipientRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(UnspamMailCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.IsSpam = false;
            return true;
        }
    }
}
