using MailCore.Application.Interfaces.Persistence;
using MailCore.Application.Services;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MailCore.Application.Tests.Services;

public class AttachmentServiceTests
{
    private readonly Mock<IAttachmentRepository> _attachmentRepo = new();
    private readonly Mock<IFileStorage> _fileStorage = new();
    private readonly AttachmentService _sut;
    private readonly Email _email = new() { Id = Guid.NewGuid() };

    public AttachmentServiceTests()
    {
        _sut = new AttachmentService(_attachmentRepo.Object, _fileStorage.Object);
    }

    private static IFormFile MakeFile(string name, long sizeBytes)
    {
   var mock = new Mock<IFormFile>();
      mock.Setup(f => f.FileName).Returns(name);
      mock.Setup(f => f.Length).Returns(sizeBytes);
      mock.Setup(f => f.ContentType).Returns("application/pdf");
     mock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[sizeBytes]));
   return mock.Object;
  }

    [Fact]
    public async Task AddAsync_NullEmail_Throws()
    {
     await Assert.ThrowsAsync<ArgumentNullException>(
  () => _sut.AddAsync(null!, [], default));
    }

    [Fact]
    public async Task AddAsync_EmptyFiles_DoesNothing()
  {
   await _sut.AddAsync(_email, [], default);
   _fileStorage.Verify(s => s.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task AddAsync_ValidFile_SavesAndPersistsAttachment()
    {
   var file = MakeFile("doc.pdf", 1024);
    _fileStorage.Setup(s => s.SaveAsync(It.IsAny<Stream>(), "doc.pdf", "application/pdf", default))
    .ReturnsAsync("storage-key-123");

        await _sut.AddAsync(_email, [file], default);

    _fileStorage.Verify(s => s.SaveAsync(It.IsAny<Stream>(), "doc.pdf", "application/pdf", default), Times.Once);
     _attachmentRepo.Verify(r => r.AddAsync(It.Is<Attachment>(a =>
 a.EmailId == _email.Id &&
     a.FileName == "doc.pdf" &&
    a.StorageKey == "storage-key-123"), default), Times.Once);
    }

    [Fact]
    public async Task AddAsync_FileExceedsLimit_Throws()
    {
var bigFile = MakeFile("huge.zip", 11 * 1024 * 1024); // 11 MB

  await Assert.ThrowsAsync<ArgumentException>(
 () => _sut.AddAsync(_email, [bigFile], default));

      _fileStorage.Verify(s => s.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), default), Times.Never);
  }

    [Fact]
  public async Task AddAsync_MultipleFiles_SavesAll()
    {
  var file1 = MakeFile("a.pdf", 512);
  var file2 = MakeFile("b.pdf", 512);
    _fileStorage.Setup(s => s.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), default))
  .ReturnsAsync("key");

   await _sut.AddAsync(_email, [file1, file2], default);

  _attachmentRepo.Verify(r => r.AddAsync(It.IsAny<Attachment>(), default), Times.Exactly(2));
    }
}
