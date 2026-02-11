using MailService.Domain.Entities;
using MailService.Infrastructure.Repositories;

namespace MailService.Infrastructure.Tests.Repositories;

public class DraftRepositoryTests : RepositoryTestBase
{
    private static readonly DateTime FixedNow = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private readonly DraftRepository _sut;

    public DraftRepositoryTests()
    {
        _sut = new DraftRepository(Context);
    }

    [Fact]
    public async Task AddAsync_And_GetByIdAsync_Persists()
    {
        var user = await SeedUserAsync();
        var draft = new Draft
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Subject = "Draft Subject",
            Body = "Draft Body",
            UpdatedAt = FixedNow
        };

        await _sut.AddAsync(draft);
        await SaveAndDetachAsync();

        var result = await _sut.GetByIdAsync(draft.Id);

        Assert.NotNull(result);
        Assert.Equal("Draft Subject", result!.Subject);
        Assert.Equal(user.Id, result.UserId);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsNull()
    {
        Assert.Null(await _sut.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdateAsync_ExistingDraft_UpdatesFields()
    {
        var user = await SeedUserAsync();
        var draft = new Draft
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Subject = "Old",
            Body = "Old Body",
            UpdatedAt = FixedNow.AddHours(-1)
        };
        Context.Drafts.Add(draft);
        await SaveAndDetachAsync();

        var updated = new Draft { Subject = "New", Body = "New Body", UpdatedAt = FixedNow };

        var result = await _sut.UpdateAsync(draft.Id, updated);

        Assert.Equal("New", result.Subject);
        Assert.Equal("New Body", result.Body);
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var updated = new Draft { Subject = "X", Body = "Y", UpdatedAt = FixedNow };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
             _sut.UpdateAsync(Guid.NewGuid(), updated));
    }

    [Fact]
    public async Task DeleteAsync_ExistingDraft_RemovesFromDb()
    {
        var user = await SeedUserAsync();
        var draft = new Draft
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Subject = "To Delete",
            Body = "Body",
            UpdatedAt = FixedNow
        };
        Context.Drafts.Add(draft);
        await SaveAndDetachAsync();

        await _sut.DeleteAsync(draft.Id);
        await SaveAndDetachAsync();

        Assert.Null(await _sut.GetByIdAsync(draft.Id));
    }

    [Fact]
    public async Task DeleteAsync_NonExistent_DoesNotThrow()
    {
        // Should be a no-op
        await _sut.DeleteAsync(Guid.NewGuid());
        await SaveAndDetachAsync();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyUsersDrafts_OrderedByDate()
    {
        var alice = await SeedUserAsync();
        var bob = await SeedUserAsync();

        Context.Drafts.AddRange(
            new Draft { Id = Guid.NewGuid(), UserId = alice.Id, Subject = "Old", Body = "B", UpdatedAt = FixedNow.AddHours(-2) },
            new Draft { Id = Guid.NewGuid(), UserId = alice.Id, Subject = "New", Body = "B", UpdatedAt = FixedNow.AddHours(-1) },
            new Draft { Id = Guid.NewGuid(), UserId = bob.Id, Subject = "Bob's", Body = "B", UpdatedAt = FixedNow }
        );
        await SaveAndDetachAsync();

        var result = await _sut.GetAllAsync(alice.Id);

        Assert.Equal(2, result.Count);
        Assert.Equal("New", result[0].Subject);   // most recent first
        Assert.Equal("Old", result[1].Subject);
    }

    [Fact]
    public async Task GetAllAsync_NoDrafts_ReturnsEmptyList()
    {
        var user = await SeedUserAsync();

        var result = await _sut.GetAllAsync(user.Id);

        Assert.Empty(result);
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
