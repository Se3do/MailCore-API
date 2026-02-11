using MailService.Domain.Entities;
using MailService.Domain.Enums;
using MailService.Infrastructure.Repositories;

namespace MailService.Infrastructure.Tests.Repositories;

/// <summary>
/// Tests for <see cref="MailRecipientRepository"/>.
/// Note: List-query methods (GetInboxAsync, GetDeletedAsync, etc.) use circular Includes
/// with <c>AsNoTracking()</c> which the EF Core InMemory provider does not support.
/// Those query-filtering paths are covered by the <c>MailboxServiceTests</c> via Moq.
/// Here we focus on the tracking-based methods: Add, GetById, GetByUserAndEmail.
/// </summary>
public class MailRecipientRepositoryTests : RepositoryTestBase
{
    private static readonly DateTime FixedNow = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private readonly MailRecipientRepository _sut;

    public MailRecipientRepositoryTests()
    {
        _sut = new MailRecipientRepository(Context);
    }

    [Fact]
    public async Task AddAsync_And_GetByIdAsync_Persists()
    {
        var (sender, recipient, thread) = await SeedBaseDataAsync();
        var email = await SeedEmailAsync(sender, thread);
        var mr = new MailRecipient
        {
            Id = Guid.NewGuid(),
            UserId = recipient.Id,
            EmailId = email.Id,
            Type = RecipientType.To,
            IsRead = false,
            IsStarred = false,
            IsSpam = false,
            ReceivedAt = FixedNow
        };

        await _sut.AddAsync(mr);
        await SaveAndDetachAsync();

        var result = await _sut.GetByIdAsync(mr.Id);

        Assert.NotNull(result);
        Assert.Equal(recipient.Id, result!.UserId);
        Assert.Equal(email.Id, result.EmailId);
        Assert.Equal(RecipientType.To, result.Type);
        Assert.False(result.IsRead);
    }

    [Fact]
    public async Task GetByIdAsync_IncludesEmailAndSender()
    {
        var (sender, recipient, thread) = await SeedBaseDataAsync();
        var email = await SeedEmailAsync(sender, thread);
        var mr = new MailRecipient
        {
            Id = Guid.NewGuid(),
            UserId = recipient.Id,
            EmailId = email.Id,
            Type = RecipientType.To,
            ReceivedAt = FixedNow
        };
        Context.MailRecipients.Add(mr);
        await SaveAndDetachAsync();

        var result = await _sut.GetByIdAsync(mr.Id);

        Assert.NotNull(result);
        Assert.NotNull(result!.Email);
        Assert.Equal("Test Subject", result.Email.Subject);
        Assert.NotNull(result.Email.Sender);
        Assert.Equal(sender.Email, result.Email.Sender.Email);
    }

    [Fact]
    public async Task GetByIdAsync_IncludesLabels()
    {
        var (sender, recipient, thread) = await SeedBaseDataAsync();
        var email = await SeedEmailAsync(sender, thread);
        var label = new Label { Id = Guid.NewGuid(), UserId = recipient.Id, Name = "Work", Color = "blue" };
        Context.Labels.Add(label);

        var mr = new MailRecipient
        {
            Id = Guid.NewGuid(),
            UserId = recipient.Id,
            EmailId = email.Id,
            Type = RecipientType.To,
            ReceivedAt = FixedNow
        };
        Context.MailRecipients.Add(mr);
        Context.MailRecipientLabels.Add(new MailRecipientLabel { MailRecipientId = mr.Id, LabelId = label.Id });
        await SaveAndDetachAsync();

        var result = await _sut.GetByIdAsync(mr.Id);

        Assert.NotNull(result);
        Assert.Single(result!.Labels);
        Assert.Equal("Work", result.Labels.First().Label.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsNull()
    {
        Assert.Null(await _sut.GetByIdAsync(Guid.NewGuid()));
    }

    // ?? GetByUserAndEmailAsync ?????????????????????????????????????????

    [Fact]
    public async Task GetByUserAndEmailAsync_ExistingEntry_ReturnsMailRecipient()
    {
        var (sender, recipient, thread) = await SeedBaseDataAsync();
        var email = await SeedEmailAsync(sender, thread);
        var mr = new MailRecipient
        {
            Id = Guid.NewGuid(),
            UserId = recipient.Id,
            EmailId = email.Id,
            Type = RecipientType.To,
            ReceivedAt = FixedNow
        };
        Context.MailRecipients.Add(mr);
        await SaveAndDetachAsync();

        var result = await _sut.GetByUserAndEmailAsync(recipient.Id, email.Id);

        Assert.NotNull(result);
        Assert.Equal(recipient.Id, result!.UserId);
        Assert.Equal(email.Id, result.EmailId);
    }

    [Fact]
    public async Task GetByUserAndEmailAsync_WrongUser_ReturnsNull()
    {
        var (sender, recipient, thread) = await SeedBaseDataAsync();
        var email = await SeedEmailAsync(sender, thread);
        var mr = new MailRecipient
        {
            Id = Guid.NewGuid(),
            UserId = recipient.Id,
            EmailId = email.Id,
            Type = RecipientType.To,
            ReceivedAt = FixedNow
        };
        Context.MailRecipients.Add(mr);
        await SaveAndDetachAsync();

        var result = await _sut.GetByUserAndEmailAsync(sender.Id, email.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserAndEmailAsync_WrongEmail_ReturnsNull()
    {
        var (sender, recipient, thread) = await SeedBaseDataAsync();
        var email = await SeedEmailAsync(sender, thread);
        var mr = new MailRecipient
        {
            Id = Guid.NewGuid(),
            UserId = recipient.Id,
            EmailId = email.Id,
            Type = RecipientType.To,
            ReceivedAt = FixedNow
        };
        Context.MailRecipients.Add(mr);
        await SaveAndDetachAsync();

        var result = await _sut.GetByUserAndEmailAsync(recipient.Id, Guid.NewGuid());

        Assert.Null(result);
    }

    // ?? Multiple recipients per email ??????????????????????????????????

    [Fact]
    public async Task AddAsync_MultipleRecipientsForSameEmail_AllPersisted()
    {
        var (sender, recipient1, thread) = await SeedBaseDataAsync();
        var recipient2 = await SeedUserAsync($"cc-{Guid.NewGuid():N}@test.com");
        var email = await SeedEmailAsync(sender, thread);

        var mr1 = new MailRecipient { Id = Guid.NewGuid(), UserId = recipient1.Id, EmailId = email.Id, Type = RecipientType.To, ReceivedAt = FixedNow };
        var mr2 = new MailRecipient { Id = Guid.NewGuid(), UserId = recipient2.Id, EmailId = email.Id, Type = RecipientType.Cc, ReceivedAt = FixedNow };

        await _sut.AddAsync(mr1);
        await _sut.AddAsync(mr2);
        await SaveAndDetachAsync();

        var result1 = await _sut.GetByIdAsync(mr1.Id);
        var result2 = await _sut.GetByIdAsync(mr2.Id);

        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(RecipientType.To, result1!.Type);
        Assert.Equal(RecipientType.Cc, result2!.Type);
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

    private async Task<Email> SeedEmailAsync(User sender, Domain.Entities.Thread thread)
    {
        var email = new Email
        {
            Id = Guid.NewGuid(),
            SenderId = sender.Id,
            Subject = "Test Subject",
            Body = "Test Body",
            CreatedAt = FixedNow,
            ThreadId = thread.Id
        };
        Context.Emails.Add(email);
        await Context.SaveChangesAsync();
        return email;
    }

    private async Task<(User sender, User recipient, Domain.Entities.Thread thread)> SeedBaseDataAsync()
    {
        var sender = await SeedUserAsync($"sender-{Guid.NewGuid():N}@test.com");
        var recipient = await SeedUserAsync($"recipient-{Guid.NewGuid():N}@test.com");
        var thread = await SeedThreadAsync();
        return (sender, recipient, thread);
    }
}
