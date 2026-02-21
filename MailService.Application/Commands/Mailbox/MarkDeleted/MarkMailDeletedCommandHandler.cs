using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Commands.Mailbox.MarkDeleted
{
    public class MarkMailDeletedCommandHandler: IRequestHandler<MarkMailDeletedCommand, bool>
    {
        private readonly IMailRecipientRepository _repo;

        public MarkMailDeletedCommandHandler(IMailRecipientRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(MarkMailDeletedCommand cmd, CancellationToken ct)
        {
            var mr = await _repo.GetByIdAsync(cmd.MailId, ct);
            if (mr is null || mr.UserId != cmd.UserId)
                return false;

            mr.DeletedAt = DateTime.UtcNow;
            return true;
        }
    }
}
