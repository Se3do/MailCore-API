using MailCore.Application.Interfaces.Persistence;

namespace MailCore.Infrastructure.Storage
{
    public sealed class LocalFileStorage : IFileStorage
    {
        private readonly string _rootPath;

        public LocalFileStorage(string rootPath)
        {
            _rootPath = rootPath;
            Directory.CreateDirectory(_rootPath);
        }

        public async Task<string> SaveAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            var extension = Path.GetExtension(fileName);
            var storageKey = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(_rootPath, storageKey);

            try
            {
                // Use FileStream with async options for better reliability
                await using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous);
                await content.CopyToAsync(fs, cancellationToken);
                await fs.FlushAsync(cancellationToken);

                return storageKey;
            }
            catch (Exception ex)
            {
                // rethrow with context so caller can log / inspect
                throw new IOException($"Failed saving file to '{fullPath}': {ex.Message}", ex);
            }
        }

        public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
        {
            var fullPath = Path.Combine(_rootPath, storageKey);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        public Task<Stream> GetAsync(string storageKey, CancellationToken cancellationToken = default)
        {
            var fullPath = Path.Combine(_rootPath, storageKey);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not found", fullPath);

            Stream stream = File.OpenRead(fullPath);
            return Task.FromResult(stream);
        }
    }
}
