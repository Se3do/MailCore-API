using MailCore.Application.DTOs.Emails;
using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Emails.ReplyEmail
{
    public record ReplyEmailCommand(Guid UserId, Guid EmailId, ReplyEmailRequest Request) : IRequest, ICommand;
}
