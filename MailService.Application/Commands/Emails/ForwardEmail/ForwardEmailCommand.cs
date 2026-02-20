using MailService.Application.DTOs.Emails;

namespace MailService.Application.Commands.Emails.ForwardEmail
{
    public record ForwardEmailCommand(Guid UserId, Guid EmailId, ForwardEmailRequest Request);
}
