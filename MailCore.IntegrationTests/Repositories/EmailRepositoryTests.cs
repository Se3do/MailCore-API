using MailCore.Domain.Common;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailCore.Infrastructure.Repositories;
using MailCore.IntegrationTests.Fixtures;

namespace MailCore.IntegrationTests.Repositories;

public class EmailRepositoryTests : IClassFixture<MailCoreDbFixture>
{
    private readonly MailCoreDbFixture _fixture;

    public EmailRepositoryTests(MailCoreDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task SearchPagedAsync_MatchesSubjectAndBody()
    {
        using var context = _fixture.CreateContext();
        var repo = new EmailRepository(context);
        var sender = await SeedUserAsync(context);
        var recipient = await SeedUserAsync(context);
        var thread = await SeedThreadAsync(context);
        var email = Email.Create(sender.Id, "Quarterly Roadmap", "Discuss Q2 priorities", threadId: thread.Id, createdAt: DateTime.UtcNow, id: Guid.NewGuid());
        context.Emails.Add(email);
        context.MailRecipients.Add(MailRecipient.Create(recipient.Id, email.Id, RecipientType.To, DateTime.UtcNow));
        await context.SaveChangesAsync();
        await _fixture.WaitForFullTextIndexAsync();

        var cursor = new Cursor(DateTime.MaxValue, Guid.Empty);
        var bySubject = await repo.SearchPagedAsync(recipient.Id, "Roadmap", cursor, 10);
        var byBody = await repo.SearchPagedAsync(recipient.Id, "priorities", cursor, 10);
        var noMatch = await repo.SearchPagedAsync(recipient.Id, "nonexistentterm", cursor, 10);

        Assert.Single(bySubject);
        Assert.Single(byBody);
        Assert.Empty(noMatch);
    }

    [Fact]
    public async Task SearchPagedAsync_MatchesSenderEmail()
    {
        using var context = _fixture.CreateContext();
        var repo = new EmailRepository(context);
        var sender = await SeedUserAsync(context);
        var recipient = await SeedUserAsync(context);
        var thread = await SeedThreadAsync(context);
        var email = Email.Create(sender.Id, "Subject", "Body", threadId: thread.Id, createdAt: DateTime.UtcNow, id: Guid.NewGuid());
        context.Emails.Add(email);
        context.MailRecipients.Add(MailRecipient.Create(recipient.Id, email.Id, RecipientType.To, DateTime.UtcNow));
        await context.SaveChangesAsync();
        await _fixture.WaitForFullTextIndexAsync();

        var cursor = new Cursor(DateTime.MaxValue, Guid.Empty);
        var result = await repo.SearchPagedAsync(recipient.Id, sender.Email, cursor, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task SearchPagedAsync_UsesPrefixMatch()
    {
        using var context = _fixture.CreateContext();
        var repo = new EmailRepository(context);
        var sender = await SeedUserAsync(context);
        var recipient = await SeedUserAsync(context);
        var thread = await SeedThreadAsync(context);
        var email = Email.Create(sender.Id, "Quarterly Roadmap", "Body", threadId: thread.Id, createdAt: DateTime.UtcNow, id: Guid.NewGuid());
        context.Emails.Add(email);
        context.MailRecipients.Add(MailRecipient.Create(recipient.Id, email.Id, RecipientType.To, DateTime.UtcNow));
        await context.SaveChangesAsync();
        await _fixture.WaitForFullTextIndexAsync();

        var cursor = new Cursor(DateTime.MaxValue, Guid.Empty);
        var result = await repo.SearchPagedAsync(recipient.Id, "Road", cursor, 10);

        Assert.Single(result);
    }

    private async Task<User> SeedUserAsync(MailCore.Infrastructure.Data.Context.MailCoreDbContext context)
    {
        var user = User.Create("Test User", $"test-{Guid.NewGuid():N}@test.com", "hash");
        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    private async Task<Domain.Entities.Thread> SeedThreadAsync(MailCore.Infrastructure.Data.Context.MailCoreDbContext context)
    {
        var thread = Domain.Entities.Thread.Create(createdAt: DateTime.UtcNow, lastMessageAt: DateTime.UtcNow, id: Guid.NewGuid());
        context.Threads.Add(thread);
        await context.SaveChangesAsync();
        return thread;
    }
}
