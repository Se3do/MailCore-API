namespace MailCore.Application.Models;

public sealed class FileData
{
    public string FileName { get; }
    public string ContentType { get; }
    public long Length { get; }
    public Func<Stream> OpenReadStream { get; }

    public FileData(string fileName, string contentType, long length, Func<Stream> openReadStream)
    {
        FileName = fileName;
        ContentType = contentType;
        Length = length;
        OpenReadStream = openReadStream;
    }
}
