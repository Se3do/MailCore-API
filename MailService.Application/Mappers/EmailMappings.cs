using MailService.Application.DTOs.Attachments;
using MailService.Application.DTOs.Emails;
using MailService.Application.DTOs.Labels;
using MailService.Domain.Entities;
using MailService.Domain.Enums;

namespace MailService.Application.Mappers;

public static class EmailMappings
{
    public static EmailSummaryDto ToSummaryDto(this Email email)
    {
        return new EmailSummaryDto(
            email.Id,
            email.Subject,
            CreatePreview(email.Body),
            email.Sender.Email,
            new DateTimeOffset(email.CreatedAt),
            email.ThreadId,
            email.HasAttachments);
    }

    public static EmailDto ToDto(this Email email, IReadOnlyList<LabelDto>? labels = null)
    {
        var to = MapRecipients(email.Recipients, RecipientType.To, allowNull: false) ?? Array.Empty<string>();
        var cc = MapRecipients(email.Recipients, RecipientType.Cc, allowNull: true);
        var bcc = MapRecipients(email.Recipients, RecipientType.Bcc, allowNull: true);

        IReadOnlyList<AttachmentDto> attachments = email.Attachments
            .Select(a => a.ToDto())
            .ToList();

        return new EmailDto(
            email.Id,
            email.Subject,
            email.Body,
            email.Sender.Email,
            to,
            cc,
            bcc,
            new DateTimeOffset(email.CreatedAt),
            email.ThreadId,
            attachments,
            labels ?? Array.Empty<LabelDto>());
    }

    private static IReadOnlyList<string>? MapRecipients(IEnumerable<MailRecipient> recipients, RecipientType type, bool allowNull)
    {
        var values = recipients
            .Where(r => r.Type == type)
            .Select(r => r.User?.Email)
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .ToList();

        if (values.Count == 0)
        {
            return allowNull ? null : Array.Empty<string>();
        }

        return values;
    }

    private static string CreatePreview(string body, int maxLength = 100)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return string.Empty;
        }

        return body.Length <= maxLength
            ? body
            : body[..maxLength];
    }
}
