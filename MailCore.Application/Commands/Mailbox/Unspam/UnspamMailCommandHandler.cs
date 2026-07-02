using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Commands.Mailbox.Unspam;

public sealed class UnspamMailCommandHandler(IMailRecipientRepository repo)
    : BaseToggleMailHandler<UnspamMailCommand>(repo)
{
    protected override void ApplyToggle(MailRecipient r) => r.SetSpam(false);
}
