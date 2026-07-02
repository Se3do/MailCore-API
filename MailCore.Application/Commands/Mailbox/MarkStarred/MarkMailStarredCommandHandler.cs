using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Commands.Mailbox.MarkStarred;

public sealed class MarkMailStarredCommandHandler(IMailRecipientRepository repo)
    : BaseToggleMailHandler<MarkMailStarredCommand>(repo)
{
    protected override void ApplyToggle(MailRecipient r) => r.SetStarred(true);
}
