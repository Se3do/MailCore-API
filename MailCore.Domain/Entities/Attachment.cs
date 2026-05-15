namespace MailCore.Domain.Entities
{
    public class Attachment
    {
        public Guid Id { get; private set; }

        public Guid EmailId { get; private set; }
        public Email Email { get; private set; } = null!;

        public string FileName { get; private set; } = null!;
        public string ContentType { get; private set; } = null!;
        public long FileSize { get; private set; }
        public string StorageKey { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }

        public static Attachment Create(Guid emailId, string fileName, string contentType, long fileSize, string storageKey, Guid? id = null)
        {
            return new Attachment
            {
                Id = id ?? Guid.NewGuid(),
                EmailId = emailId,
                FileName = fileName,
                ContentType = contentType,
                FileSize = fileSize,
                StorageKey = storageKey,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
