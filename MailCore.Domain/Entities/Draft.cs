namespace MailCore.Domain.Entities
{
    public class Draft
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid? ThreadId { get; set; }
        public Thread? Thread { get; set; }

        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string? ToRecipients { get; set; }
        public string? CcRecipients { get; set; }
        public string? BccRecipients { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
