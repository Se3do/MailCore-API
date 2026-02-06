using MailService.Application.DTOs.Mailbox;
using MailService.Domain.Entities;

namespace MailService.Application.Mappers;

public static class MailboxMappings
{
    public static MailboxItemDto ToMailboxItemDto(this MailRecipient mailRecipient)
    {
        var email = mailRecipient.Email;

        return new MailboxItemDto(
            mailRecipient.Id,
            email.Id,
            email.Sender.Email,
            EmailMappings.ToDto(email).To,
            email.Subject,
            EmailMappings.ToSummaryDto(email).Preview,
            new DateTimeOffset(email.CreatedAt),
            mailRecipient.IsRead,
            mailRecipient.IsStarred,
            mailRecipient.IsSpam,
            mailRecipient.DeletedAt.HasValue,
            email.ThreadId);
    }

    public static MailboxDetailDto ToMailboxDetailDto(this MailRecipient mailRecipient)
    {
        var labels = mailRecipient.Labels
            .Select(l => l.Label.ToDto())
            .ToList();

        var emailDto = mailRecipient.Email.ToDto(labels);

        return new MailboxDetailDto(
            mailRecipient.Id,
            emailDto,
            mailRecipient.IsRead,
            mailRecipient.IsStarred,
            mailRecipient.IsSpam,
            mailRecipient.DeletedAt.HasValue,
            labels);
    }
}
