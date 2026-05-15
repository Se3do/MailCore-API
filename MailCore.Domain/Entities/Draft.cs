namespace MailCore.Domain.Entities
{
    public class Draft
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public Guid? ThreadId { get; private set; }
        public Thread? Thread { get; private set; }

        public string Subject { get; private set; } = null!;
        public string Body { get; private set; } = null!;
        public string? ToRecipients { get; private set; }
        public string? CcRecipients { get; private set; }
        public string? BccRecipients { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public static Draft Create(Guid userId, string subject, string body, string? to = null, string? cc = null, string? bcc = null, Guid? threadId = null, Guid? id = null)
        {
            return new Draft
            {
                Id = id ?? Guid.NewGuid(),
                UserId = userId,
                Subject = subject,
                Body = body,
                ToRecipients = to,
                CcRecipients = cc,
                BccRecipients = bcc,
                ThreadId = threadId,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public void UpdateContent(string subject, string body, string? to = null, string? cc = null, string? bcc = null)
        {
            Subject = subject;
            Body = body;
            ToRecipients = to;
            CcRecipients = cc;
            BccRecipients = bcc;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
