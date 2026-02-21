using MediatR;
using MailService.Domain.Common;

namespace MailService.Application.Commands.Mailbox.MarkRead
{
 public sealed record MarkMailReadCommand(Guid UserId, Guid MailId): IRequest<bool>, ICommand;
}
