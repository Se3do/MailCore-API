namespace MailService.Domain.Entities
{
    public class Label
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public bool IsSystemLabel { get; set; }

        public ICollection<MailRecipientLabel> MailRecipients { get; set; } = new List<MailRecipientLabel>();
    }
}
