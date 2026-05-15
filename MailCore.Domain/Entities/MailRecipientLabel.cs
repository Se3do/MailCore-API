namespace MailCore.Domain.Entities
{
    public class MailRecipientLabel
    {
        public Guid MailRecipientId { get; private set; }
        public MailRecipient MailRecipient { get; private set; } = null!;

        public Guid LabelId { get; private set; }
        public Label Label { get; private set; } = null!;

        public static MailRecipientLabel Create(Guid mailRecipientId, Guid labelId)
        {
            return new MailRecipientLabel
            {
                MailRecipientId = mailRecipientId,
                LabelId = labelId
            };
        }
    }
}
