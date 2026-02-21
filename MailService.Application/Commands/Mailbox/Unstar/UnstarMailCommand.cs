using MediatR;
using MailService.Domain.Common;

namespace MailService.Application.Commands.Mailbox.Unstar
{
    public sealed record UnstarMailCommand(Guid UserId, Guid MailId): IRequest<bool>, ICommand;
}
