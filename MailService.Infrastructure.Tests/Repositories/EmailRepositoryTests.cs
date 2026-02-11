using MailService.Domain.Entities;
using MailService.Infrastructure.Repositories;

namespace MailService.Infrastructure.Tests.Repositories;

public class EmailRepositoryTests : RepositoryTestBase
{
    private static readonly DateTime FixedNow = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private readonly EmailRepository _sut;

    public EmailRepositoryTests()
    {
        _sut = new EmailRepository(Context);
    }

    [Fact]
    public async Task AddAsync_And_GetByIdAsync_RoundTrips()
    {
        // Arrange
        var sender = await SeedUserAsync("alice@test.com");
        var email = new Email
        {
            Id = Guid.NewGuid(),
            SenderId = sender.Id,
            Subject = "Hello",
            Body = "World",
            CreatedAt = FixedNow,
            ThreadId = (await SeedThreadAsync()).Id
        };

        // Act
        await _sut.AddAsync(email);
        await SaveAndDetachAsync();

        var result = await _sut.GetByIdAsync(email.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Hello", result!.Subject);
        Assert.Equal("World", result.Body);
        Assert.Equal(sender.Id, result.SenderId);
        Assert.Equal("alice@test.com", result.Sender.Email);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_IncludesRecipientsAndAttachments()
    {
        // Arrange
        var sender = await SeedUserAsync("sender@test.com");
        var recipient = await SeedUserAsync("recipient@test.com");
        var thread = await SeedThreadAsync();
        var email = new Email
        {
            Id = Guid.NewGuid(),
            SenderId = sender.Id,
            Subject = "With relations",
            Body = "Body",
            CreatedAt = FixedNow,
            ThreadId = thread.Id,
            HasAttachments = true
        };
        Context.Emails.Add(email);

        Context.MailRecipients.Add(new MailRecipient
        {
            Id = Guid.NewGuid(),
            UserId = recipient.Id,
            EmailId = email.Id,
            Type = Domain.Enums.RecipientType.To,
            ReceivedAt = FixedNow
        });
        Context.Attachments.Add(new Attachment
        {
            Id = Guid.NewGuid(),
            EmailId = email.Id,
            FileName = "doc.pdf",
            ContentType = "application/pdf",
            FileSize = 1024,
            FilePath = "/files/doc.pdf",
            UploadedAt = FixedNow
        });
        await SaveAndDetachAsync();

        // Act
        var result = await _sut.GetByIdAsync(email.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result!.Recipients);
        Assert.Equal("recipient@test.com", result.Recipients.First().User.Email);
        Assert.Single(result.Attachments);
        Assert.Equal("doc.pdf", result.Attachments.First().FileName);
    }

    [Fact]
    public async Task GetSentAsync_ReturnsOnlyEmailsSentByUser()
    {
        // Arrange
        var alice = await SeedUserAsync("alice@test.com");
        var bob = await SeedUserAsync("bob@test.com");
        var thread = await SeedThreadAsync();

        Context.Emails.AddRange(
            new Email { Id = Guid.NewGuid(), SenderId = alice.Id, Subject = "From Alice 1", Body = "B", CreatedAt = FixedNow, ThreadId = thread.Id },
            new Email { Id = Guid.NewGuid(), SenderId = alice.Id, Subject = "From Alice 2", Body = "B", CreatedAt = FixedNow.AddMinutes(1), ThreadId = thread.Id },
            new Email { Id = Guid.NewGuid(), SenderId = bob.Id, Subject = "From Bob", Body = "B", CreatedAt = FixedNow, ThreadId = thread.Id }
        );
        await SaveAndDetachAsync();

        // Act
        var result = await _sut.GetSentAsync(alice.Id);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.Equal(alice.Id, e.SenderId));
    }

    [Fact]
    public async Task GetSentAsync_NoEmails_ReturnsEmptyList()
    {
        var user = await SeedUserAsync("lonely@test.com");

        var result = await _sut.GetSentAsync(user.Id);

        Assert.Empty(result);
    }

    private async Task<User> SeedUserAsync(string email)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = email.Split('@')[0],
            Email = email,
            PasswordHash = "hash",
            CreatedAt = FixedNow
        };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();
        return user;
    }

    private async Task<Domain.Entities.Thread> SeedThreadAsync()
    {
        var thread = new Domain.Entities.Thread
        {
            Id = Guid.NewGuid(),
            CreatedAt = FixedNow,
            LastMessageAt = FixedNow
        };
        Context.Threads.Add(thread);
        await Context.SaveChangesAsync();
        return thread;
    }
}
