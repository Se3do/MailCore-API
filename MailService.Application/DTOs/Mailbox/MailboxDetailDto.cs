using MailService.Application.DTOs.Emails;
using MailService.Application.DTOs.Labels;

namespace MailService.Application.DTOs.Mailbox;

public sealed record MailboxDetailDto(
    Guid MailRecipientId,
    EmailDto Email,
    bool IsRead,
    bool IsStarred,
    bool IsSpam,
    bool IsTrash,
    IReadOnlyList<LabelDto> Labels
);
