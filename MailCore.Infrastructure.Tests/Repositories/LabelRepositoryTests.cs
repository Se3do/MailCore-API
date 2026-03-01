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
        var label = new Label
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = "Important",
            Color = "#ff0000"
        };

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
        var label = new Label { Id = Guid.NewGuid(), UserId = user.Id, Name = "Old", Color = "blue" };
        Context.Labels.Add(label);
        await SaveAndDetachAsync();

        var updated = new Label { Name = "New", Color = "green", IsSystemLabel = true };
        var result = await _sut.UpdateAsync(label.Id, updated);

        Assert.Equal("New", result.Name);
        Assert.Equal("green", result.Color);
        Assert.True(result.IsSystemLabel);
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ThrowsKeyNotFoundException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.UpdateAsync(Guid.NewGuid(), new Label { Name = "X", Color = "Y" }));
    }

    [Fact]
    public async Task DeleteAsync_ExistingLabel_RemovesFromDb()
    {
        var user = await SeedUserAsync();
        var label = new Label { Id = Guid.NewGuid(), UserId = user.Id, Name = "ToDelete", Color = "red" };
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
             new Label { Id = Guid.NewGuid(), UserId = alice.Id, Name = "Work", Color = "blue" },
            new Label { Id = Guid.NewGuid(), UserId = alice.Id, Name = "Personal", Color = "green" },
          new Label { Id = Guid.NewGuid(), UserId = bob.Id, Name = "BobLabel", Color = "red" }
        );
        await SaveAndDetachAsync();

        var result = await _sut.GetAllAsync(alice.Id);

        Assert.Equal(2, result.Count);
        Assert.All(result, l => Assert.Equal(alice.Id, l.UserId));
    }

    private async Task<User> SeedUserAsync()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = $"user-{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@test.com",
            PasswordHash = "hash",
            CreatedAt = FixedNow
        };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();
        return user;
    }
}
