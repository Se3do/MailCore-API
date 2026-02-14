namespace MailService.Infrastructure.Storage
{
    public sealed class FileStorageOptions
    {
        public string Provider { get; set; } = "Local";
        public bool UseProjectDirectory { get; set; }
        public string? RelativePath { get; set; }
        public string? RootPath { get; set; }
    }

}
