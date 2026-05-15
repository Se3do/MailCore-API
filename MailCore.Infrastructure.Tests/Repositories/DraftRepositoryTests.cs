using MailCore.Domain.Entities;
using MailCore.Infrastructure.Repositories;
using Xunit;

namespace MailCore.Infrastructure.Tests.Repositories;

public class DraftRepositoryTests : RepositoryTestBase
{
    private readonly DraftRepository _sut;

    public DraftRepositoryTests()
    {
        _sut = new DraftRepository(Context);
    }

    [Fact]
    public async Task AddAsync_And_GetByIdAsync_PersistsDraft()
    {
        var draft = Draft.Create(Guid.NewGuid(), "Subject", "Body", id: Guid.NewGuid());

        var user = User.Create("Test", "test@test.com", "hash");
        user.Id = draft.UserId;
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        await _sut.AddAsync(draft);
        await SaveAndDetachAsync();

        var result = await _sut.GetByIdAsync(draft.Id);
        Assert.NotNull(result);
        Assert.Equal("Subject", result.Subject);
    }

    private static void SetField<T>(T target, string propertyName, object value)
    {
        var field = typeof(T).GetField($"<{propertyName}>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field!.SetValue(target, value);
    }
}
