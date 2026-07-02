using System.Reflection;
using MailCore.Application.Common.Pagination;
using MailCore.Application.Queries.Mailbox;
using MailCore.Domain.Common;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailCore.Domain.Interfaces;
using Moq;

namespace MailCore.Application.Tests.Queries.Mailbox;

public class MailboxPagedQueryHandlerTests
{
    private sealed record TestPagedQuery(Guid UserId, CursorPaginationQuery Pagination) : IMailboxPagedQuery;

    private sealed class TestPagedQueryHandler(IMailRecipientRepository repo)
        : BaseMailboxPagedQueryHandler<TestPagedQuery>(repo)
    {
        public Func<IMailRecipientRepository, TestPagedQuery, CancellationToken, Task<IReadOnlyList<MailRecipient>>>? FetchImpl { get; set; }

        protected override Task<IReadOnlyList<MailRecipient>> FetchAsync(IMailRecipientRepository repo, TestPagedQuery query, CancellationToken ct)
            => FetchImpl!(repo, query, ct);
    }

    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var userId = Guid.NewGuid();
        var pagination = new CursorPaginationQuery(null, 10);
        var repo = new Mock<IMailRecipientRepository>();

        var email = MailCore.Domain.Entities.Email.Create(Guid.NewGuid(), "Test", "Hello", threadId: Guid.NewGuid(), createdAt: DateTime.UtcNow);
        SetPrivateField(email, "Sender", User.Create("", "s@s.com", ""));
        var mr = MailRecipient.Create(userId, email.Id, RecipientType.To, DateTime.UtcNow);
        SetPrivateField(mr, "Email", email);
        var mails = new List<MailRecipient> { mr };

        var handler = new TestPagedQueryHandler(repo.Object)
        {
            FetchImpl = (r, q, ct) => r.GetInboxPagedAsync(q.UserId, q.Pagination.ToCursor(), q.Pagination.PageSize, ct)
        };

        repo.Setup(r => r.GetInboxPagedAsync(userId, It.IsAny<Cursor>(), 10, default))
            .ReturnsAsync(mails);

        var result = await handler.Handle(new TestPagedQuery(userId, pagination), default);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        Assert.Single(result.Items);
    }

    private static void SetPrivateField<T>(T target, string propertyName, object value)
    {
        var field = typeof(T).GetField($"<{propertyName}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
        field!.SetValue(target, value);
    }
}
