namespace MailCore.Domain.Entities
{
    public class Attachment
    {
        public Guid Id { get; set; }

        public Guid EmailId { get; set; }
        public Email Email { get; set; } = null!;

        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long FileSize { get; set; }
        public string StorageKey { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
