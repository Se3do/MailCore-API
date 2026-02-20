using MailService.Application.DTOs.Emails;
using MediatR;

namespace MailService.Application.Commands.Emails.SendEmail
{
    public record SendEmailCommand(Guid UserId, SendEmailRequest Request): IRequest;
}