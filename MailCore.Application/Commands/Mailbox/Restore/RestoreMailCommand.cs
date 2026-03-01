using MediatR;
using MailCore.Domain.Common;

namespace MailCore.Application.Commands.Mailbox.Restore
{
    public sealed record RestoreMailCommand(Guid UserId, Guid MailId): IRequest<bool>, ICommand;
}
