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

        public void SetRead(bool value) => IsRead = value;

        public void SetStarred(bool value) => IsStarred = value;

        public void SetSpam(bool value) => IsSpam = value;

        public void SetDeleted(bool value) => DeletedAt = value ? DateTime.UtcNow : null;
    }
}
