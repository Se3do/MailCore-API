using MailCore.Infrastructure.Storage;
using Microsoft.AspNetCore.Http;
using Xunit;
using System.Text;

namespace MailCore.Infrastructure.Tests.Storage;

public class LocalFileStorageTests
{
    private readonly string _tempDirectory;
    private readonly LocalFileStorage _sut;

    public LocalFileStorageTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _sut = new LocalFileStorage(_tempDirectory);
    }

    [Fact]
    public async Task SaveAsync_SavesFileAndReturnsKey()
    {
        var content = "Hello World";
        var bytes = Encoding.UTF8.GetBytes(content);
        using var ms = new MemoryStream(bytes);

        var resultKey = await _sut.SaveAsync(ms, "test.txt", "text/plain");

        Assert.NotNull(resultKey);
        var fullPath = Path.Combine(_tempDirectory, resultKey);
        Assert.True(File.Exists(fullPath));

        var savedContent = await File.ReadAllTextAsync(fullPath);
        Assert.Equal(content, savedContent);

        if (Directory.Exists(_tempDirectory))
            Directory.Delete(_tempDirectory, true);
    }
}
