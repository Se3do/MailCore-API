using MailCore.Application.Exceptions;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Mailbox.MarkDeleted
{
    public class MarkMailDeletedCommandHandler : IRequestHandler<MarkMailDeletedCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;

        public MarkMailDeletedCommandHandler(IMailRecipientRepository repo) => _repo = repo;

        public async Task<bool> Handle(MarkMailDeletedCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct)
                ?? throw new NotFoundException($"Mail {cmd.MailId} not found.");

            if (mr.UserId != cmd.UserId)
                throw new ForbiddenException("You do not have access to this mail.");

            mr.DeletedAt = DateTime.UtcNow;
            return true;
        }
    }
}
