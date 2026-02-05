using MailService.Domain.Enums;

namespace MailService.Domain.Entities
{
    public class MailRecipient
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid EmailId { get; set; }
        public Email Email { get; set; } = null!;

        public RecipientType Type { get; set; }

        public bool IsRead { get; set; }
        public bool IsSpam { get; set; }
        public bool IsStarred { get; set; }

        public DateTime ReceivedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<MailRecipientLabel> Labels { get; set; } = new List<MailRecipientLabel>();
    }
}
