using MediatR;
using MailCore.Domain.Common;

namespace MailCore.Application.Commands.Mailbox.MarkStarred
{
    public sealed record MarkMailStarredCommand(Guid UserId, Guid MailId): IRequest<bool>, ICommand;
}
