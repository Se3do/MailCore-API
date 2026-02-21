using MediatR;
using MailService.Domain.Common;

namespace MailService.Application.Commands.Mailbox.MarkUnread
{
    public sealed record MarkMailUnreadCommand(Guid UserId, Guid MailId): IRequest<bool>, ICommand;
}
