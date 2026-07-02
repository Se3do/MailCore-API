using MediatR;

namespace MailCore.Application.Commands.Mailbox;

public interface IMailboxToggleCommand : IRequest<bool>
{
    Guid UserId { get; }
    Guid MailId { get; }
}
