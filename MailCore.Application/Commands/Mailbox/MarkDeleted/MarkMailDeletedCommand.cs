using MailCore.Domain.Common;

namespace MailCore.Application.Commands.Mailbox.MarkDeleted
{
 public sealed record MarkMailDeletedCommand(Guid UserId, Guid MailId): IMailboxToggleCommand, ICommand;
}
