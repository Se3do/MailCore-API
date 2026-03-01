using MediatR;
using MailCore.Domain.Common;

namespace MailCore.Application.Commands.Mailbox.MarkUnread
{
    public sealed record MarkMailUnreadCommand(Guid UserId, Guid MailId): IRequest<bool>, ICommand;
}
