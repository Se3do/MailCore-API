using MailService.Application.DTOs.Emails;

namespace MailService.Application.Commands.Emails.ReplyEmail
{
    public record ReplyEmailCommand(Guid UserId, Guid EmailId, ReplyEmailRequest Request);
}
