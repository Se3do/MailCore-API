using MailCore.Application.Exceptions;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Mailbox.MarkUnread
{
    public class MarkMailUnreadCommandHandler : IRequestHandler<MarkMailUnreadCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;
        public MarkMailUnreadCommandHandler(IMailRecipientRepository repo) => _repo = repo;

        public async Task<bool> Handle(MarkMailUnreadCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct)
                    ?? throw new NotFoundException($"Mail {cmd.MailId} not found.");

            if (mr.UserId != cmd.UserId)
                throw new ForbiddenException("You do not have access to this mail.");

            mr.IsRead = false;
            return true;
        }
    }
}
