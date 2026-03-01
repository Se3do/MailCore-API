namespace MailCore.Application.Interfaces.Persistence
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default);
        Task<Stream> GetAsync(string storageKey, CancellationToken cancellationToken = default);
        Task DeleteAsync(string filePath, CancellationToken cancellationToken = default);
    }
}