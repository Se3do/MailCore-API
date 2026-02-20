using MailService.Application.DTOs.Emails;
using MediatR;

namespace MailService.Application.Commands.Emails.ReplyEmail
{
    public record ReplyEmailCommand(Guid UserId, Guid EmailId, ReplyEmailRequest Request) : IRequest;
}
