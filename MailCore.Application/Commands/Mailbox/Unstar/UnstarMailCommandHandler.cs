using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Mailbox.Unstar
{
    public class UnstarMailCommandHandler: IRequestHandler<UnstarMailCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;

        public UnstarMailCommandHandler(IMailRecipientRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(UnstarMailCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.IsStarred = false;
            return true;
        }
    }
}
