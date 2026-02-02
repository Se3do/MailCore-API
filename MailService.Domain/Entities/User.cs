namespace MailService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public ICollection<Email> SentEmails { get; set; } = new List<Email>();
        public ICollection<MailRecipient> MailRecipients { get; set; } = new List<MailRecipient>();
        public ICollection<Label> Labels { get; set; } = new List<Label>();
        public ICollection<Draft> Drafts { get; set; } = new List<Draft>();
    }
}
