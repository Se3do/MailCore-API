using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Commands.Mailbox.Restore;

public sealed class RestoreMailCommandHandler(IMailRecipientRepository repo)
    : BaseToggleMailHandler<RestoreMailCommand>(repo)
{
    protected override void ApplyToggle(MailRecipient r) => r.SetDeleted(false);
}
