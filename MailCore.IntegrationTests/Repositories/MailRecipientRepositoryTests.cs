using MailCore.Domain.Common;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailCore.Infrastructure.Repositories;
using MailCore.IntegrationTests.Fixtures;

namespace MailCore.IntegrationTests.Repositories;

public class MailRecipientRepositoryTests : IClassFixture<MailCoreDbFixture>
{
    private readonly MailCoreDbFixture _fixture;

    public MailRecipientRepositoryTests(MailCoreDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetInboxPagedAsync_ReturnsNonSpamNonDeleted()
    {
        using var context = _fixture.CreateContext();
        var repo = new MailRecipientRepository(context);
        var user = await SeedUserAsync(context);
        var thread = await SeedThreadAsync(context);
        var email = await SeedEmailAsync(context, user, thread);
        var spamEmail = await SeedEmailAsync(context, user, thread);
        var deletedEmail = await SeedEmailAsync(context, user, thread);

        await SeedMailRecipientAsync(context, email.Id, user.Id, RecipientType.To, isSpam: false, deletedAt: null);
        await SeedMailRecipientAsync(context, spamEmail.Id, user.Id, RecipientType.To, isSpam: true, deletedAt: null);
        await SeedMailRecipientAsync(context, deletedEmail.Id, user.Id, RecipientType.To, isSpam: false, deletedAt: DateTime.UtcNow);

        await context.SaveChangesAsync();

        var cursor = new Cursor(DateTime.MaxValue, Guid.Empty);
        var result = await repo.GetInboxPagedAsync(user.Id, cursor, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetStarredPagedAsync_ReturnsOnlyStarred()
    {
        using var context = _fixture.CreateContext();
        var repo = new MailRecipientRepository(context);
        var user = await SeedUserAsync(context);
        var thread = await SeedThreadAsync(context);
        var email1 = await SeedEmailAsync(context, user, thread);
        var email2 = await SeedEmailAsync(context, user, thread);

        await SeedMailRecipientAsync(context, email1.Id, user.Id, RecipientType.To, isStarred: true);
        await SeedMailRecipientAsync(context, email2.Id, user.Id, RecipientType.To, isStarred: false);

        await context.SaveChangesAsync();

        var cursor = new Cursor(DateTime.MaxValue, Guid.Empty);
        var result = await repo.GetStarredPagedAsync(user.Id, cursor, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetUnreadPagedAsync_ReturnsOnlyUnread()
    {
        using var context = _fixture.CreateContext();
        var repo = new MailRecipientRepository(context);
        var user = await SeedUserAsync(context);
        var thread = await SeedThreadAsync(context);
        var email1 = await SeedEmailAsync(context, user, thread);
        var email2 = await SeedEmailAsync(context, user, thread);

        await SeedMailRecipientAsync(context, email1.Id, user.Id, RecipientType.To, isRead: false);
        await SeedMailRecipientAsync(context, email2.Id, user.Id, RecipientType.To, isRead: true);

        await context.SaveChangesAsync();

        var cursor = new Cursor(DateTime.MaxValue, Guid.Empty);
        var result = await repo.GetUnreadPagedAsync(user.Id, cursor, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetSpamPagedAsync_ReturnsOnlySpam()
    {
        using var context = _fixture.CreateContext();
        var repo = new MailRecipientRepository(context);
        var user = await SeedUserAsync(context);
        var thread = await SeedThreadAsync(context);
        var email1 = await SeedEmailAsync(context, user, thread);
        var email2 = await SeedEmailAsync(context, user, thread);

        await SeedMailRecipientAsync(context, email1.Id, user.Id, RecipientType.To, isSpam: true);
        await SeedMailRecipientAsync(context, email2.Id, user.Id, RecipientType.To, isSpam: false);

        await context.SaveChangesAsync();

        var cursor = new Cursor(DateTime.MaxValue, Guid.Empty);
        var result = await repo.GetSpamPagedAsync(user.Id, cursor, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetDeletedPagedAsync_ReturnsOnlyDeleted()
    {
        using var context = _fixture.CreateContext();
        var repo = new MailRecipientRepository(context);
        var user = await SeedUserAsync(context);
        var thread = await SeedThreadAsync(context);
        var email1 = await SeedEmailAsync(context, user, thread);
        var email2 = await SeedEmailAsync(context, user, thread);

        await SeedMailRecipientAsync(context, email1.Id, user.Id, RecipientType.To, deletedAt: DateTime.UtcNow);
        await SeedMailRecipientAsync(context, email2.Id, user.Id, RecipientType.To, deletedAt: null);

        await context.SaveChangesAsync();

        var cursor = new Cursor(DateTime.MaxValue, Guid.Empty);
        var result = await repo.GetDeletedPagedAsync(user.Id, cursor, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByLabelPagedAsync_ReturnsOnlyLabelled()
    {
        using var context = _fixture.CreateContext();
        var repo = new MailRecipientRepository(context);
        var user = await SeedUserAsync(context);
        var thread = await SeedThreadAsync(context);
        var email1 = await SeedEmailAsync(context, user, thread);
        var email2 = await SeedEmailAsync(context, user, thread);

        var label = new Label { Id = Guid.NewGuid(), UserId = user.Id, Name = "Test", Color = "red" };
        context.Labels.Add(label);

        var mr1 = await SeedMailRecipientAsync(context, email1.Id, user.Id, RecipientType.To);
        var mr2 = await SeedMailRecipientAsync(context, email2.Id, user.Id, RecipientType.To);

        context.MailRecipientLabels.Add(new MailRecipientLabel { MailRecipientId = mr1.Id, LabelId = label.Id });

        await context.SaveChangesAsync();

        var cursor = new Cursor(DateTime.MaxValue, Guid.Empty);
        var result = await repo.GetByLabelPagedAsync(user.Id, label.Id, cursor, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task CursorPagination_RespectsPageSize()
    {
        using var context = _fixture.CreateContext();
        var repo = new MailRecipientRepository(context);
        var user = await SeedUserAsync(context);
        var thread = await SeedThreadAsync(context);

        for (int i = 0; i < 5; i++)
        {
            var email = await SeedEmailAsync(context, user, thread);
            await SeedMailRecipientAsync(context, email.Id, user.Id, RecipientType.To);
        }

        await context.SaveChangesAsync();

        var cursor = new Cursor(DateTime.MaxValue, Guid.Empty);
        var result = await repo.GetInboxPagedAsync(user.Id, cursor, 3);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task CursorPagination_ReturnsCorrectOrder()
    {
        using var context = _fixture.CreateContext();
        var repo = new MailRecipientRepository(context);
        var user = await SeedUserAsync(context);
        var thread = await SeedThreadAsync(context);

        var email1 = await SeedEmailAsync(context, user, thread);
        await Task.Delay(10);
        var email2 = await SeedEmailAsync(context, user, thread);
        await Task.Delay(10);
        var email3 = await SeedEmailAsync(context, user, thread);

        var mr1 = await SeedMailRecipientAsync(context, email1.Id, user.Id, RecipientType.To);
        var mr2 = await SeedMailRecipientAsync(context, email2.Id, user.Id, RecipientType.To);
        var mr3 = await SeedMailRecipientAsync(context, email3.Id, user.Id, RecipientType.To);

        await context.SaveChangesAsync();

        var cursor = new Cursor(DateTime.MaxValue, Guid.Empty);
        var result = await repo.GetInboxPagedAsync(user.Id, cursor, 10);

        Assert.Equal(3, result.Count);
        Assert.True(result[0].ReceivedAt >= result[1].ReceivedAt);
        Assert.True(result[1].ReceivedAt >= result[2].ReceivedAt);
    }

    private async Task<User> SeedUserAsync(MailCore.Infrastructure.Data.Context.MailCoreDbContext context)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = $"test-{Guid.NewGuid():N}@test.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    private async Task<Domain.Entities.Thread> SeedThreadAsync(MailCore.Infrastructure.Data.Context.MailCoreDbContext context)
    {
        var thread = new Domain.Entities.Thread
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            LastMessageAt = DateTime.UtcNow
        };
        context.Threads.Add(thread);
        await context.SaveChangesAsync();
        return thread;
    }

    private async Task<Email> SeedEmailAsync(
        MailCore.Infrastructure.Data.Context.MailCoreDbContext context,
        User sender,
        Domain.Entities.Thread thread)
    {
        var email = new Email
        {
            Id = Guid.NewGuid(),
            SenderId = sender.Id,
            ThreadId = thread.Id,
            Subject = "Test Subject",
            Body = "Test Body",
            CreatedAt = DateTime.UtcNow
        };
        context.Emails.Add(email);
        await context.SaveChangesAsync();
        return email;
    }

    private Task<MailRecipient> SeedMailRecipientAsync(
        MailCore.Infrastructure.Data.Context.MailCoreDbContext context,
        Guid emailId,
        Guid userId,
        RecipientType type,
        bool isRead = false,
        bool isSpam = false,
        bool isStarred = false,
        DateTime? deletedAt = null)
    {
        var mr = new MailRecipient
        {
            Id = Guid.NewGuid(),
            EmailId = emailId,
            UserId = userId,
            Type = type,
            IsRead = isRead,
            IsSpam = isSpam,
            IsStarred = isStarred,
            DeletedAt = deletedAt,
            ReceivedAt = DateTime.UtcNow
        };
        context.MailRecipients.Add(mr);
        return Task.FromResult(mr);
    }
}
