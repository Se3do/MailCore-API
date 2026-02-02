namespace MailService.Domain.Entities
{
    public class MailRecipientLabel
    {
        public Guid MailRecipientId { get; set; }
        public MailRecipient MailRecipient { get; set; } = null!;

        public Guid LabelId { get; set; }
        public Label Label { get; set; } = null!;
    }
}
