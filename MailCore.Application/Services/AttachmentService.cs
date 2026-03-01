using MailCore.Application.Interfaces.Persistence;
using MailCore.Application.Interfaces.Services;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MailCore.Application.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IFileStorage _fileStorage;

        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        public AttachmentService(
            IAttachmentRepository attachmentRepository,
            IFileStorage fileStorage)
        {
            _attachmentRepository = attachmentRepository;
            _fileStorage = fileStorage;
        }

        public async Task AddAsync(
            Email email,
            IReadOnlyCollection<IFormFile> files,
            CancellationToken cancellationToken)
        {
            if (email is null)
                throw new ArgumentNullException(nameof(email));

            if (files is null || files.Count == 0)
                return;

            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (file is null || file.Length == 0)
                    continue;

                if (file.Length > MaxFileSizeBytes)
                    throw new ArgumentException(
                        $"Attachment '{file.FileName}' exceeds the maximum allowed size.");

                var fileName = Path.GetFileName(file.FileName);
                var contentType = string.IsNullOrWhiteSpace(file.ContentType)
                    ? "application/octet-stream"
                    : file.ContentType;

                await using var stream = file.OpenReadStream();

                var storageKey = await _fileStorage.SaveAsync(
                    stream,
                    fileName,
                    contentType,
                    cancellationToken);

                var attachment = new Attachment
                {
                    Id = Guid.NewGuid(),
                    EmailId = email.Id,
                    FileName = fileName,
                    ContentType = contentType,
                    FileSize = file.Length,
                    StorageKey = storageKey,
                    CreatedAt = DateTime.UtcNow
                };

                await _attachmentRepository.AddAsync(attachment, cancellationToken);
            }
        }
    }

}
