using MailCore.Domain.Entities;
using MailCore.Infrastructure.Repositories;
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
        var user = new User { Id = Guid.NewGuid(), Email = "s@s.com", Name = "S", PasswordHash = "hash" };
        var thread = new Domain.Entities.Thread { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, LastMessageAt = DateTime.UtcNow };
        
        Context.Users.Add(user);
        Context.Threads.Add(thread);
        await Context.SaveChangesAsync();

        var email = new Domain.Entities.Email
        {
            Id = Guid.NewGuid(),
            SenderId = user.Id,
            ThreadId = thread.Id,
            Subject = "Hello",
            Body = "World",
            CreatedAt = DateTime.UtcNow
        };

        await _sut.AddAsync(email);
        await SaveAndDetachAsync();

        var result = await _sut.GetByIdAsync(email.Id);
        Assert.NotNull(result);
        Assert.Equal("Hello", result.Subject);
    }
}
