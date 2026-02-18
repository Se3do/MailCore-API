using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Emails;
using MailService.Application.DTOs.Mailbox;

namespace MailService.Application.Services.Interfaces;

public interface IEmailService
{
    Task<EmailDto> SendAsync(Guid userId, SendEmailRequest request, CancellationToken cancellationToken = default);
    Task<EmailDto> ReplyAsync(Guid userId, Guid emailId, ReplyEmailRequest request, CancellationToken cancellationToken = default);
    Task<EmailDto> ForwardAsync(Guid userId, Guid emailId, ForwardEmailRequest request, CancellationToken cancellationToken = default);
}
