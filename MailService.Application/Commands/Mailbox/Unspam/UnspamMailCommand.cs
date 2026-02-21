using MediatR;
using MailService.Domain.Common;

namespace MailService.Application.Commands.Mailbox.Unspam
{
    public sealed record UnspamMailCommand(Guid UserId, Guid MailId): IRequest<bool>, ICommand;
}
