using MailCore.Domain.Entities;

namespace MailCore.Infrastructure.MailSender
{
    public interface IEmailSender
    {
        Task SendAsync(Email email, IReadOnlyList<MailRecipient> recipients, CancellationToken cancellationToken = default);
    }
}
