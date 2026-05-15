using MailCore.Domain.Entities;
using MailCore.Infrastructure.Repositories;
using ThreadEntity = MailCore.Domain.Entities.Thread;
using Xunit;

namespace MailCore.Infrastructure.Tests.Repositories;

public class ThreadRepositoryTests : RepositoryTestBase
{
    private readonly ThreadRepository _sut;

    public ThreadRepositoryTests()
    {
        _sut = new ThreadRepository(Context);
    }

    [Fact]
    public async Task AddAsync_And_GetByIdAsync_PersistsThread()
    {
        var thread = ThreadEntity.Create(createdAt: DateTime.UtcNow, lastMessageAt: DateTime.UtcNow, id: Guid.NewGuid());

        await _sut.AddAsync(thread);
        await SaveAndDetachAsync();

        var result = await _sut.GetByIdAsync(thread.Id);
        Assert.NotNull(result);
    }

    private static void SetField<T>(T target, string propertyName, object value)
    {
        var field = typeof(T).GetField($"<{propertyName}>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field!.SetValue(target, value);
    }
}
