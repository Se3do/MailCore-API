using MailService.Application.DTOs.Emails;

namespace MailService.Application.Services.Interfaces;

public interface IEmailService
{
    Task<EmailDto> SendAsync(Guid userId, SendEmailRequest request, CancellationToken cancellationToken = default);
    Task<EmailDto> ReplyAsync(Guid userId, Guid emailId, ReplyEmailRequest request, CancellationToken cancellationToken = default);
    Task<EmailDto> ForwardAsync(Guid userId, Guid emailId, ForwardEmailRequest request, CancellationToken cancellationToken = default);
}
