using MediatR;
using MailService.Domain.Common;

namespace MailService.Application.Commands.Mailbox.Restore
{
    public sealed record RestoreMailCommand(Guid UserId, Guid MailId): IRequest<bool>, ICommand;
}
