using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Commands.Mailbox.Unstar;

public sealed class UnstarMailCommandHandler(IMailRecipientRepository repo)
    : BaseToggleMailHandler<UnstarMailCommand>(repo)
{
    protected override void ApplyToggle(MailRecipient r) => r.SetStarred(false);
}
