using MailService.Application.DTOs.Emails;
using MediatR;

namespace MailService.Application.Commands.Emails.ForwardEmail
{
    public record ForwardEmailCommand(Guid UserId, Guid EmailId, ForwardEmailRequest Request) : IRequest;
}
