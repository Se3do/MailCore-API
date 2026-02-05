namespace MailService.Application.Interfaces.Persistence
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(byte[] content, string fileName, string contentType, CancellationToken cancellationToken = default);
        Task DeleteAsync(string filePath, CancellationToken cancellationToken = default);
    }
}