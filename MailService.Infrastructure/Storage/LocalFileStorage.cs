using MailService.Application.Interfaces.Persistence;

namespace MailService.Infrastructure.Storage
{
    public sealed class LocalFileStorage : IFileStorage
    {
        private readonly string _rootPath;

        public LocalFileStorage(string rootPath)
        {
            _rootPath = rootPath;
            Directory.CreateDirectory(_rootPath);
        }

        public async Task<string> SaveAsync(byte[] content, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            var extension = Path.GetExtension(fileName);
            var safeName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(_rootPath, safeName);

            await File.WriteAllBytesAsync(fullPath, content, cancellationToken);
            return fullPath;
        }

        public Task DeleteAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
    }
}