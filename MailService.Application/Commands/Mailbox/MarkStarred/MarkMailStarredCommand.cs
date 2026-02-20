using MediatR;

namespace MailService.Application.Commands.Mailbox.MarkStarred
{
    public sealed record MarkMailStarredCommand(Guid UserId, Guid MailId): IRequest<bool>;
}
