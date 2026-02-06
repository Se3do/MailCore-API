namespace MailService.Application.DTOs.Mailbox;

public sealed record MailboxItemDto(
    Guid MailRecipientId,
    Guid EmailId,
    string From,
    IReadOnlyList<string> To,
    string Subject,
    string Preview,
    DateTimeOffset SentAt,
    bool IsRead,
    bool IsStarred,
    bool IsSpam,
    bool IsTrash,
    Guid? ThreadId
);
