using MailCore.Application.DTOs.Emails;
using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Emails.SendEmail
{
    public record SendEmailCommand(Guid UserId, SendEmailRequest Request): IRequest, ICommand;
}