using MailCore.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace MailCore.Application.Interfaces.Services
{
    public interface IAttachmentService
    {
        Task AddAsync(Email email, IReadOnlyCollection<IFormFile> files, CancellationToken cancellationToken);
    }

}
