namespace MailCore.Application.DTOs.Mailbox;

/// <summary>Summary of an email in a mailbox folder view.</summary>
public sealed record MailboxItemDto(
    Guid MailRecipientId,
    Guid EmailId,
    string From,
    string Subject,
    string Preview,
    DateTimeOffset SentAt,
    bool IsRead,
    bool IsStarred,
    bool IsSpam,
    bool IsTrash,
    Guid? ThreadId
);
