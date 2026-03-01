using MailCore.Application.DTOs.Emails;
using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Emails.ForwardEmail
{
    public record ForwardEmailCommand(Guid UserId, Guid EmailId, ForwardEmailRequest Request) : IRequest, ICommand;
}
