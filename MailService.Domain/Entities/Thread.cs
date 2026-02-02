namespace MailService.Domain.Entities
{
    public class Thread
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastMessageAt { get; set; }

        public ICollection<Email> Emails { get; set; } = new List<Email>();
    }
}
