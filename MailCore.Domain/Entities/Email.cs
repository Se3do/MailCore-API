using MailCore.Domain.Enums;

namespace MailCore.Domain.Entities
{
    public class Email
    {
        public Guid Id { get; private set; }

        public Guid ThreadId { get; private set; }
        public Thread Thread { get; private set; } = null!;

        public Guid SenderId { get; private set; }
        public User Sender { get; private set; } = null!;

        public string Subject { get; private set; } = null!;
        public string Body { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public bool HasAttachments { get; private set; }
        public EmailDeliveryStatus DeliveryStatus { get; private set; }
        public DateTime? SentAt { get; private set; }
        public int SendAttempts { get; private set; }
        public string? LastSendError { get; private set; }

        public ICollection<Attachment> Attachments { get; private set; } = new List<Attachment>();
        public ICollection<MailRecipient> Recipients { get; private set; } = new List<MailRecipient>();

        public static Email Create(Guid senderId, string subject, string body, Guid? threadId = null, DateTime? createdAt = null, Guid? id = null)
        {
            return new Email
            {
                Id = id ?? Guid.NewGuid(),
                ThreadId = threadId ?? Guid.NewGuid(),
                SenderId = senderId,
                Subject = subject,
                Body = body,
                CreatedAt = createdAt ?? DateTime.UtcNow,
                DeliveryStatus = EmailDeliveryStatus.Pending,
                HasAttachments = false,
                SendAttempts = 0
            };
        }

        public void MarkAsSent(DateTime? sentAt = null)
        {
            DeliveryStatus = EmailDeliveryStatus.Sent;
            SentAt = sentAt ?? DateTime.UtcNow;
        }

        public void MarkAsFailed(string error)
        {
            SendAttempts++;
            LastSendError = error;
            if (SendAttempts >= 3)
                DeliveryStatus = EmailDeliveryStatus.Failed;
        }

        public void AttachFiles()
        {
            HasAttachments = true;
        }
    }
}
