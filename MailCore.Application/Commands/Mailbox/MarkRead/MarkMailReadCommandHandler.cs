using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Commands.Mailbox.MarkRead;

public sealed class MarkMailReadCommandHandler(IMailRecipientRepository repo)
    : BaseToggleMailHandler<MarkMailReadCommand>(repo)
{
    protected override void ApplyToggle(MailRecipient r) => r.SetRead(true);
}
