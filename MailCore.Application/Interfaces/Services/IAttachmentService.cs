using MailCore.Application.Models;
using MailCore.Domain.Entities;

namespace MailCore.Application.Interfaces.Services
{
    public interface IAttachmentService
    {
        Task AddAsync(Email email, IReadOnlyCollection<FileData> files, CancellationToken cancellationToken);
    }

}
