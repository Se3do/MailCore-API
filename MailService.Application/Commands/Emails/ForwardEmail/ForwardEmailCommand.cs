using MailService.Application.DTOs.Emails;
using MailService.Domain.Common;
using MediatR;

namespace MailService.Application.Commands.Emails.ForwardEmail
{
    public record ForwardEmailCommand(Guid UserId, Guid EmailId, ForwardEmailRequest Request) : IRequest, ICommand;
}
