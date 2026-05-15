using MailCore.Domain.Entities;
using MailCore.Infrastructure.Repositories;
using ThreadEntity = MailCore.Domain.Entities.Thread;
using Xunit;

namespace MailCore.Infrastructure.Tests.Repositories;

public class EmailRepositoryTests : RepositoryTestBase
{
    private readonly EmailRepository _sut;

    public EmailRepositoryTests()
    {
        _sut = new EmailRepository(Context);
    }

    [Fact]
    public async Task AddAsync_And_GetByIdAsync_PersistsEmail()
    {
        var user = User.Create("S", "s@s.com", "hash");
        user.Id = Guid.NewGuid();
        var thread = ThreadEntity.Create(createdAt: DateTime.UtcNow, lastMessageAt: DateTime.UtcNow, id: Guid.NewGuid());
        
        Context.Users.Add(user);
        Context.Threads.Add(thread);
        await Context.SaveChangesAsync();

        var email = Email.Create(user.Id, "Hello", "World", threadId: thread.Id, createdAt: DateTime.UtcNow, id: Guid.NewGuid());

        await _sut.AddAsync(email);
        await SaveAndDetachAsync();

        var result = await _sut.GetByIdAsync(email.Id);
        Assert.NotNull(result);
        Assert.Equal("Hello", result.Subject);
    }

    private static void SetField<T>(T target, string propertyName, object value)
    {
        var field = typeof(T).GetField($"<{propertyName}>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field!.SetValue(target, value);
    }
}
