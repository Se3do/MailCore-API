using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Emails;
using MailService.Application.DTOs.Mailbox;

namespace MailService.Application.Services.Interfaces;

public interface IEmailService
{
    Task SendAsync(Guid userId, SendEmailRequest request, CancellationToken cancellationToken = default);
    Task ReplyAsync(Guid userId, Guid emailId, ReplyEmailRequest request, CancellationToken cancellationToken = default);
    Task ForwardAsync(Guid userId, Guid emailId, ForwardEmailRequest request, CancellationToken cancellationToken = default);
    Task<CursorPagedResult<EmailSummaryDto>> GetSentPagedAsync(Guid userId, CursorPaginationQuery query, CancellationToken ct);
    Task<EmailDto?> GetSentByIdAsync(Guid userId, Guid emailId, CancellationToken ct);
}
