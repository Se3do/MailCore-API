using MailCore.Domain.Entities;
using MailCore.Infrastructure.Repositories;

namespace MailCore.Infrastructure.Tests.Repositories;

public class LabelRepositoryTests : RepositoryTestBase
{
    private static readonly DateTime FixedNow = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private readonly LabelRepository _sut;

    public LabelRepositoryTests()
    {
        _sut = new LabelRepository(Context);
    }

    [Fact]
    public async Task AddAsync_And_GetByIdAsync_Persists()
    {
        var user = await SeedUserAsync();
        var label = Label.Create(user.Id, "Important", "#ff0000", id: Guid.NewGuid());

        await _sut.AddAsync(label);
        await SaveAndDetachAsync();

        var result = await _sut.GetByIdAsync(label.Id);

        Assert.NotNull(result);
        Assert.Equal("Important", result!.Name);
        Assert.Equal("#ff0000", result.Color);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsNull()
    {
        Assert.Null(await _sut.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdateAsync_ExistingLabel_UpdatesFields()
    {
        var user = await SeedUserAsync();
        var label = Label.Create(user.Id, "Old", "blue", id: Guid.NewGuid());
        Context.Labels.Add(label);
        await SaveAndDetachAsync();

        var updated = Label.Create(Guid.Empty, "New", "green", isSystem: true);
        var result = await _sut.UpdateAsync(label.Id, updated);

        Assert.Equal("New", result.Name);
        Assert.Equal("green", result.Color);
        Assert.True(result.IsSystemLabel);
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ThrowsKeyNotFoundException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.UpdateAsync(Guid.NewGuid(), Label.Create(Guid.Empty, "X", "Y")));
    }

    [Fact]
    public async Task DeleteAsync_ExistingLabel_RemovesFromDb()
    {
        var user = await SeedUserAsync();
        var label = Label.Create(user.Id, "ToDelete", "red", id: Guid.NewGuid());
        Context.Labels.Add(label);
        await SaveAndDetachAsync();

        await _sut.DeleteAsync(label.Id);
        await SaveAndDetachAsync();

        Assert.Null(await _sut.GetByIdAsync(label.Id));
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ThrowsKeyNotFoundException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
      _sut.DeleteAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyUsersLabels()
    {
        var alice = await SeedUserAsync();
        var bob = await SeedUserAsync();

        Context.Labels.AddRange(
             Label.Create(alice.Id, "Work", "blue", id: Guid.NewGuid()),
            Label.Create(alice.Id, "Personal", "green", id: Guid.NewGuid()),
          Label.Create(bob.Id, "BobLabel", "red", id: Guid.NewGuid())
        );
        await SaveAndDetachAsync();

        var result = await _sut.GetAllAsync(alice.Id);

        Assert.Equal(2, result.Count);
        Assert.All(result, l => Assert.Equal(alice.Id, l.UserId));
    }

    private async Task<User> SeedUserAsync()
    {
        var user = User.Create(
            $"user-{Guid.NewGuid():N}",
            $"{Guid.NewGuid():N}@test.com",
            "hash"
        );
        user.Id = Guid.NewGuid();
        user.CreatedAt = FixedNow;
        Context.Users.Add(user);
        await Context.SaveChangesAsync();
        return user;
    }

    private static void SetField<T>(T target, string propertyName, object value)
    {
        var field = typeof(T).GetField($"<{propertyName}>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field!.SetValue(target, value);
    }
}
