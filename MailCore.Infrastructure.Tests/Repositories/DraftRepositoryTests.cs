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
        var draft = new Draft
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Subject = "Subject",
            Body = "Body",
            ThreadId = null,
            UpdatedAt = DateTime.UtcNow
        };

        var user = new User { Id = draft.UserId, Email = "test@test.com", Name = "Test", PasswordHash = "hash" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        await _sut.AddAsync(draft);
        await SaveAndDetachAsync();

        var result = await _sut.GetByIdAsync(draft.Id);
        Assert.NotNull(result);
        Assert.Equal("Subject", result.Subject);
    }


}
