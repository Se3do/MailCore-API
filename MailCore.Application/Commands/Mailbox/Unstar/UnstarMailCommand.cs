using MailCore.Domain.Common;

namespace MailCore.Application.Commands.Mailbox.Unstar
{
    public sealed record UnstarMailCommand(Guid UserId, Guid MailId): IMailboxToggleCommand, ICommand;
}
