using MediatR;
using MailCore.Domain.Common;

namespace MailCore.Application.Commands.Mailbox.MarkRead
{
 public sealed record MarkMailReadCommand(Guid UserId, Guid MailId): IRequest<bool>, ICommand;
}
