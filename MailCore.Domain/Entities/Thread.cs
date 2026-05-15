namespace MailCore.Domain.Entities
{
    public class Thread
    {
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastMessageAt { get; private set; }

        public ICollection<Email> Emails { get; private set; } = new List<Email>();

        public static Thread Create(DateTime? createdAt = null, DateTime? lastMessageAt = null, Guid? id = null)
        {
            var now = createdAt ?? DateTime.UtcNow;
            return new Thread
            {
                Id = id ?? Guid.NewGuid(),
                CreatedAt = now,
                LastMessageAt = lastMessageAt ?? now
            };
        }

        public void Touch() => LastMessageAt = DateTime.UtcNow;
    }
}
