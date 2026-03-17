using MailCore.Domain.Entities;
using MailCore.Infrastructure.Repositories;
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
        var thread = new Domain.Entities.Thread
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            LastMessageAt = DateTime.UtcNow
        };

        await _sut.AddAsync(thread);
        await SaveAndDetachAsync();

        var result = await _sut.GetByIdAsync(thread.Id);
        Assert.NotNull(result);
    }
}
