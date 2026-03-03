using MailCore.Application.Exceptions;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Mailbox.Unstar
{
    public class UnstarMailCommandHandler : IRequestHandler<UnstarMailCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;
        public UnstarMailCommandHandler(IMailRecipientRepository repo) => _repo = repo;

        public async Task<bool> Handle(UnstarMailCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct)
                      ?? throw new NotFoundException($"Mail {cmd.MailId} not found.");

            if (mr.UserId != cmd.UserId)
                throw new ForbiddenException("You do not have access to this mail.");

            mr.IsStarred = false;
            return true;
        }
    }
}
