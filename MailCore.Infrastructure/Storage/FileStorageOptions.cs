namespace MailCore.Infrastructure.Storage
{
    public sealed class FileStorageOptions
    {
        public bool UseProjectDirectory { get; set; }
        public string? RelativePath { get; set; }
        public string? RootPath { get; set; }
    }

}
