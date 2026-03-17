using MailCore.Domain.Entities;
using MailCore.Infrastructure.Repositories;
using Xunit;

namespace MailCore.Infrastructure.Tests.Repositories;

public class AttachmentRepositoryTests : RepositoryTestBase
{
    private readonly AttachmentRepository _sut;

    public AttachmentRepositoryTests()
    {
        _sut = new AttachmentRepository(Context);
    }

    [Fact]
    public async Task AddAsync_PersistsAttachment()
    {
        var attachment = new Attachment
        {
            Id = Guid.NewGuid(),
            EmailId = Guid.NewGuid(),
            FileName = "test.txt",
            ContentType = "text/plain",
            StorageKey = "some-key"
        };

        var contextEmail = new Email 
        { 
            Id = attachment.EmailId, 
            Subject = "Subject", 
            Body = "Body",
            SenderId = Guid.NewGuid(), 
            CreatedAt = DateTime.UtcNow 
        };
        Context.Emails.Add(contextEmail);
        await Context.SaveChangesAsync();

        await _sut.AddAsync(attachment);
        await SaveAndDetachAsync();

        var result = await Context.Attachments.FindAsync(attachment.Id);
        Assert.NotNull(result);
        Assert.Equal("test.txt", result.FileName);
    }
}
