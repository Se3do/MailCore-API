using MediatR;
using MailService.Domain.Common;

namespace MailService.Application.Commands.Mailbox.MarkSpam
{
    public sealed record MarkMailSpamCommand(Guid UserId, Guid MailId): IRequest<bool>, ICommand;
}
