using MailCore.Application.Exceptions;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Mailbox;

public abstract class BaseToggleMailHandler<TCommand> : IRequestHandler<TCommand, bool>
    where TCommand : IMailboxToggleCommand
{
    private readonly IMailRecipientRepository _repo;

    protected BaseToggleMailHandler(IMailRecipientRepository repo) => _repo = repo;

    protected abstract void ApplyToggle(MailRecipient recipient);

    public async Task<bool> Handle(TCommand cmd, CancellationToken ct)
    {
        if (cmd.UserId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(cmd.UserId));
        if (cmd.MailId == Guid.Empty)
            throw new ArgumentException("MailId is required.", nameof(cmd.MailId));

        var mr = await _repo.GetByIdAsync(cmd.MailId, ct)
            ?? throw new NotFoundException($"Mail {cmd.MailId} not found.");

        if (mr.UserId != cmd.UserId)
            throw new ForbiddenException("You do not have access to this mail.");

        ApplyToggle(mr);
        return true;
    }
}
