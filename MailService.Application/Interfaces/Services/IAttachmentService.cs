using MailService.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace MailService.Application.Interfaces.Services
{
    public interface IAttachmentService
    {
        Task AddAsync(Email email, IReadOnlyCollection<IFormFile> files, CancellationToken cancellationToken);
    }

}
