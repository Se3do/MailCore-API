using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Commands.Mailbox.MarkDeleted;

public sealed class MarkMailDeletedCommandHandler(IMailRecipientRepository repo)
    : BaseToggleMailHandler<MarkMailDeletedCommand>(repo)
{
    protected override void ApplyToggle(MailRecipient r) => r.SetDeleted(true);
}
