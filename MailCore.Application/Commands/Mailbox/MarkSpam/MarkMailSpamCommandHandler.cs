using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Commands.Mailbox.MarkSpam;

public sealed class MarkMailSpamCommandHandler(IMailRecipientRepository repo)
    : BaseToggleMailHandler<MarkMailSpamCommand>(repo)
{
    protected override void ApplyToggle(MailRecipient r) => r.SetSpam(true);
}
