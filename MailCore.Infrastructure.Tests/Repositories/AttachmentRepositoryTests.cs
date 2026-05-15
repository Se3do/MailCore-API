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
        var attachment = Attachment.Create(Guid.NewGuid(), "test.txt", "text/plain", 1024, "some-key", id: Guid.NewGuid());

        var contextEmail = Email.Create(Guid.NewGuid(), "Subject", "Body", id: attachment.EmailId, createdAt: DateTime.UtcNow);
        Context.Emails.Add(contextEmail);
        await Context.SaveChangesAsync();

        await _sut.AddAsync(attachment);
        await SaveAndDetachAsync();

        var result = await Context.Attachments.FindAsync(attachment.Id);
        Assert.NotNull(result);
        Assert.Equal("test.txt", result.FileName);
    }

    private static void SetField<T>(T target, string propertyName, object value)
    {
        var field = typeof(T).GetField($"<{propertyName}>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field!.SetValue(target, value);
    }
}
