using MailCore.Domain.Entities;

namespace MailCore.Application.Interfaces.Services
{
    public interface IEmailSender
    {
        Task SendAsync(Email email, IReadOnlyList<MailRecipient> recipients, CancellationToken cancellationToken = default);
    }
}
