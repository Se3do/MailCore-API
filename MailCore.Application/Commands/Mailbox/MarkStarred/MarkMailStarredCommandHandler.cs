using MailCore.Application.Exceptions;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Mailbox.MarkStarred
{
    public class MarkMailStarredCommandHandler : IRequestHandler<MarkMailStarredCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;
public MarkMailStarredCommandHandler(IMailRecipientRepository repo) => _repo = repo;

        public async Task<bool> Handle(MarkMailStarredCommand cmd, CancellationToken ct)
        {
         var mr = await _repo.GetByIdAsync(cmd.MailId, ct)
    ?? throw new NotFoundException($"Mail {cmd.MailId} not found.");

   if (mr.UserId != cmd.UserId)
       throw new ForbiddenException("You do not have access to this mail.");

   mr.IsStarred = true;
     return true;
        }
    }
}
