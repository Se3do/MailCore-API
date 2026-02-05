using MailService.Application.DTOs.Attachment;

namespace MailService.Application.Interfaces.Services
{
    public interface IAttachmentService
    {
        Task<AttachmentDto?> GetByIdAsync(Guid attachmentId, CancellationToken cancellationToken = default);
        Task<Guid> UploadAsync(Guid emailId, AttachmentCreateDto attachment, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid attachmentId, CancellationToken cancellationToken = default);
    }
}