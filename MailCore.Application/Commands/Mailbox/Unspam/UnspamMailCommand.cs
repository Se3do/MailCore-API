using MediatR;
using MailCore.Domain.Common;

namespace MailCore.Application.Commands.Mailbox.Unspam
{
    public sealed record UnspamMailCommand(Guid UserId, Guid MailId): IRequest<bool>, ICommand;
}
