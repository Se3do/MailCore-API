using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Commands.Mailbox.MarkUnread;

public sealed class MarkMailUnreadCommandHandler(IMailRecipientRepository repo)
    : BaseToggleMailHandler<MarkMailUnreadCommand>(repo)
{
    protected override void ApplyToggle(MailRecipient r) => r.SetRead(false);
}
