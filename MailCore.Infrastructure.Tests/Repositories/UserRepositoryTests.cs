using MailCore.Domain.Entities;
using MailCore.Infrastructure.Repositories;

namespace MailCore.Infrastructure.Tests.Repositories;

public class UserRepositoryTests : RepositoryTestBase
{
    private static readonly DateTime FixedNow = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private readonly UserRepository _sut;

    public UserRepositoryTests()
    {
        _sut = new UserRepository(Context);
    }

    [Fact]
    public async Task AddAsync_And_GetByIdAsync_Persists()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Alice",
            Email = "alice@test.com",
            PasswordHash = "hash",
            CreatedAt = FixedNow
        };

        await _sut.AddAsync(user);
        await SaveAndDetachAsync();

        var result = await _sut.GetByIdAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal("Alice", result!.Name);
        Assert.Equal("alice@test.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsNull()
    {
        Assert.Null(await _sut.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByEmailAsync_ExistingEmail_ReturnsUser()
    {
        await SeedUserAsync("bob@test.com", "Bob");

        var result = await _sut.GetByEmailAsync("bob@test.com");

        Assert.NotNull(result);
        Assert.Equal("Bob", result!.Name);
    }

    [Fact]
    public async Task GetByEmailAsync_NotFound_ReturnsNull()
    {
        Assert.Null(await _sut.GetByEmailAsync("nonexistent@test.com"));
    }

    [Fact]
    public async Task ExistsByEmailAsync_ExistingEmail_ReturnsTrue()
    {
        await SeedUserAsync("exists@test.com", "Exists");

        Assert.True(await _sut.ExistsByEmailAsync("exists@test.com"));
    }

    [Fact]
    public async Task ExistsByEmailAsync_NonExistent_ReturnsFalse()
    {
        Assert.False(await _sut.ExistsByEmailAsync("nope@test.com"));
    }

    private async Task SeedUserAsync(string email, string name)
    {
        Context.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            PasswordHash = "hash",
            CreatedAt = FixedNow
        });
        await SaveAndDetachAsync();
    }
}
