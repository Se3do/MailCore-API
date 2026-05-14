using MailCore.Application.DTOs.Emails;
using MailCore.Application.DTOs.Labels;

namespace MailCore.Application.DTOs.Mailbox;

/// <summary>Full detail of an email in a mailbox, including labels and full email content.</summary>
public sealed record MailboxDetailDto(
    Guid MailRecipientId,
    EmailDto Email,
    bool IsRead,
    bool IsStarred,
    bool IsSpam,
    bool IsTrash,
    IReadOnlyList<LabelDto> Labels
);
