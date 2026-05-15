using MailCore.Application.Interfaces.Persistence;
using MailCore.Application.Interfaces.Services;
using MailCore.Application.Models;
using MailCore.Domain.Common;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IFileStorage _fileStorage;

        public AttachmentService(
            IAttachmentRepository attachmentRepository,
            IFileStorage fileStorage)
        {
            _attachmentRepository = attachmentRepository;
            _fileStorage = fileStorage;
        }

        public async Task AddAsync(
            Email email,
            IReadOnlyCollection<FileData> files,
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

                if (file.Length > DomainConstants.MaxAttachmentSizeBytes)
                    throw new ArgumentException(
                        $"Attachment '{file.FileName}' exceeds the maximum allowed size.");

                await using var stream = file.OpenReadStream();

                var storageKey = await _fileStorage.SaveAsync(
                    stream,
                    file.FileName,
                    file.ContentType,
                    cancellationToken);

                var attachment = Attachment.Create(email.Id, file.FileName, file.ContentType, file.Length, storageKey);

                await _attachmentRepository.AddAsync(attachment, cancellationToken);
            }
        }
    }

}
