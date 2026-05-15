using MailCore.Domain.Enums;

namespace MailCore.Domain.Entities
{
    public class MailRecipient
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public Guid EmailId { get; private set; }
        public Email Email { get; private set; } = null!;

        public RecipientType Type { get; private set; }

        public bool IsRead { get; private set; }
        public bool IsSpam { get; private set; }
        public bool IsStarred { get; private set; }

        public DateTime ReceivedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        public ICollection<MailRecipientLabel> Labels { get; private set; } = new List<MailRecipientLabel>();

        public static MailRecipient Create(Guid userId, Guid emailId, RecipientType type, DateTime receivedAt, Guid? id = null)
        {
            return new MailRecipient
            {
                Id = id ?? Guid.NewGuid(),
                UserId = userId,
                EmailId = emailId,
                Type = type,
                ReceivedAt = receivedAt
            };
        }

        public void MarkAsRead() => IsRead = true;

        public void MarkAsUnread() => IsRead = false;

        public void MarkAsStarred() => IsStarred = true;

        public void UnmarkAsStarred() => IsStarred = false;

        public void MarkAsSpam() => IsSpam = true;

        public void UnmarkAsSpam() => IsSpam = false;

        public void SoftDelete() => DeletedAt = DateTime.UtcNow;

        public void Restore() => DeletedAt = null;
    }
}
