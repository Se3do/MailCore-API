using MailCore.Domain.Common;

namespace MailCore.Application.Commands.Mailbox.MarkSpam
{
    public sealed record MarkMailSpamCommand(Guid UserId, Guid MailId): IMailboxToggleCommand, ICommand;
}
