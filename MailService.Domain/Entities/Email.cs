namespace MailService.Domain.Entities
{
    public class Email
    {
        public Guid Id { get; set; }

        public Guid ThreadId { get; set; }
        public Thread Thread { get; set; } = null!;

        public Guid SenderId { get; set; }
        public User Sender { get; set; } = null!;

        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool HasAttachments { get; set; }

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<MailRecipient> Recipients { get; set; } = new List<MailRecipient>();
    }
}
