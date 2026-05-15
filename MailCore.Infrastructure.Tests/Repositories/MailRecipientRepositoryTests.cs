using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailCore.Infrastructure.Repositories;
using ThreadEntity = MailCore.Domain.Entities.Thread;

namespace MailCore.Infrastructure.Tests.Repositories;

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
        var mr = MailRecipient.Create(recipient.Id, email.Id, RecipientType.To, FixedNow, id: Guid.NewGuid());

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
        var mr = MailRecipient.Create(recipient.Id, email.Id, RecipientType.To, FixedNow, id: Guid.NewGuid());
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
        var label = Label.Create(recipient.Id, "Work", "blue", id: Guid.NewGuid());
        Context.Labels.Add(label);

        var mr = MailRecipient.Create(recipient.Id, email.Id, RecipientType.To, FixedNow, id: Guid.NewGuid());
        Context.MailRecipients.Add(mr);
        Context.MailRecipientLabels.Add(MailRecipientLabel.Create(mr.Id, label.Id));
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

    [Fact]
    public async Task AddAsync_MultipleRecipientsForSameEmail_AllPersisted()
    {
        var (sender, recipient1, thread) = await SeedBaseDataAsync();
        var recipient2 = await SeedUserAsync($"cc-{Guid.NewGuid():N}@test.com");
        var email = await SeedEmailAsync(sender, thread);

        var mr1 = MailRecipient.Create(recipient1.Id, email.Id, RecipientType.To, FixedNow, id: Guid.NewGuid());
        var mr2 = MailRecipient.Create(recipient2.Id, email.Id, RecipientType.Cc, FixedNow, id: Guid.NewGuid());

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
        var user = User.Create(email.Split('@')[0], email, "hash");
        user.Id = Guid.NewGuid();
        user.CreatedAt = FixedNow;
        Context.Users.Add(user);
        await Context.SaveChangesAsync();
        return user;
    }

    private async Task<Domain.Entities.Thread> SeedThreadAsync()
    {
        var thread = ThreadEntity.Create(createdAt: FixedNow, lastMessageAt: FixedNow, id: Guid.NewGuid());
        Context.Threads.Add(thread);
        await Context.SaveChangesAsync();
        return thread;
    }

    private async Task<Email> SeedEmailAsync(User sender, Domain.Entities.Thread thread)
    {
        var email = Email.Create(sender.Id, "Test Subject", "Test Body", threadId: thread.Id, createdAt: FixedNow, id: Guid.NewGuid());
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

    private static void SetField<T>(T target, string propertyName, object value)
    {
        var field = typeof(T).GetField($"<{propertyName}>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field!.SetValue(target, value);
    }
}
