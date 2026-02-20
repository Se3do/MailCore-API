using MailService.Application.DTOs.Emails;

namespace MailService.Application.Commands.Emails.SendEmail
{
    public record SendEmailCommand(Guid UserId, SendEmailRequest Request);
}