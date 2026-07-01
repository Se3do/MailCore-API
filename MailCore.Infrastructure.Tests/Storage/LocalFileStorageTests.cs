using MailCore.Infrastructure.Storage;
using Xunit;
using System.Text;

namespace MailCore.Infrastructure.Tests.Storage;

public class LocalFileStorageTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly LocalFileStorage _sut;

    public LocalFileStorageTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _sut = new LocalFileStorage(_tempDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
            Directory.Delete(_tempDirectory, true);
    }

    [Fact]
    public void Constructor_CreatesRootDirectory()
    {
        Assert.True(Directory.Exists(_tempDirectory));
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
    }

    [Fact]
    public async Task SaveAsync_UniqueKeyPerCall()
    {
        using var content1 = new MemoryStream(Encoding.UTF8.GetBytes("First"));
        using var content2 = new MemoryStream(Encoding.UTF8.GetBytes("Second"));

        var key1 = await _sut.SaveAsync(content1, "file.txt", "text/plain");
        var key2 = await _sut.SaveAsync(content2, "file.txt", "text/plain");

        Assert.NotEqual(key1, key2);
    }

    [Fact]
    public async Task SaveAsync_EmptyContent_CreatesFile()
    {
        using var ms = new MemoryStream();

        var resultKey = await _sut.SaveAsync(ms, "empty.txt", "text/plain");

        var fullPath = Path.Combine(_tempDirectory, resultKey);
        Assert.True(File.Exists(fullPath));
        Assert.Empty(await File.ReadAllTextAsync(fullPath));
    }

    [Fact]
    public async Task SaveAsync_Cancellation_ThrowsIOException()
    {
        using var ms = new MemoryStream(Encoding.UTF8.GetBytes("data"));
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var ex = await Assert.ThrowsAsync<IOException>(
            () => _sut.SaveAsync(ms, "test.txt", "text/plain", cts.Token));
        Assert.IsType<TaskCanceledException>(ex.InnerException);
    }

    [Fact]
    public async Task GetAsync_ReturnsStream_WhenFileExists()
    {
        var content = "File content";
        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var key = await _sut.SaveAsync(ms, "get-test.txt", "text/plain");

        using var resultStream = await _sut.GetAsync(key);
        Assert.NotNull(resultStream);

        using var reader = new StreamReader(resultStream);
        var result = await reader.ReadToEndAsync();
        Assert.Equal(content, result);
    }

    [Fact]
    public async Task GetAsync_ThrowsFileNotFoundException_WhenFileDoesNotExist()
    {
        var missingKey = Guid.NewGuid().ToString("N") + ".txt";

        await Assert.ThrowsAsync<FileNotFoundException>(
            () => _sut.GetAsync(missingKey));
    }

    [Fact]
    public async Task GetAsync_CancelledToken_ReturnsStream()
    {
        using var ms = new MemoryStream(Encoding.UTF8.GetBytes("data"));
        var key = await _sut.SaveAsync(ms, "test.txt", "text/plain");
        var cts = new CancellationTokenSource();
        cts.Cancel();

        using var resultStream = await _sut.GetAsync(key, cts.Token);
        Assert.NotNull(resultStream);
    }

    [Fact]
    public async Task DeleteAsync_RemovesFile_WhenExists()
    {
        using var ms = new MemoryStream(Encoding.UTF8.GetBytes("delete me"));
        var key = await _sut.SaveAsync(ms, "to-delete.txt", "text/plain");

        var filePath = Path.Combine(_tempDirectory, key);
        Assert.True(File.Exists(filePath));

        await _sut.DeleteAsync(key);

        Assert.False(File.Exists(filePath));
    }

    [Fact]
    public async Task DeleteAsync_DoesNotThrow_WhenFileDoesNotExist()
    {
        var missingKey = Guid.NewGuid().ToString("N") + ".txt";

        await _sut.DeleteAsync(missingKey);
    }
}
