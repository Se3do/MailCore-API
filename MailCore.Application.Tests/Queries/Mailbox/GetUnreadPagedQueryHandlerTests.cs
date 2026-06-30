using System.Reflection;
using MailCore.Application.Common.Pagination;
using MailCore.Application.Queries.Mailbox.GetUnreadPaged;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailCore.Domain.Interfaces;
using Moq;
using Xunit;

namespace MailCore.Application.Tests.Queries.Mailbox;

public class GetUnreadPagedQueryHandlerTests
{
    private readonly Mock<IMailRecipientRepository> _mailRecipientRepo = new();
    private readonly GetUnreadPagedQueryHandler _sut;

    public GetUnreadPagedQueryHandlerTests()
    {
        _sut = new GetUnreadPagedQueryHandler(_mailRecipientRepo.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var userId = Guid.NewGuid();
        var pagination = new CursorPaginationQuery(null, 10);

        var email = MailCore.Domain.Entities.Email.Create(Guid.NewGuid(), "Test", "Hello", threadId: Guid.NewGuid(), createdAt: DateTime.UtcNow);
        SetPrivateField(email, "Sender", User.Create("", "s@s.com", ""));
        var mr = MailRecipient.Create(userId, email.Id, RecipientType.To, DateTime.UtcNow);
        SetPrivateField(mr, "Email", email);
        var mails = new List<MailRecipient> { mr };

        _mailRecipientRepo.Setup(r => r.GetUnreadPagedAsync(userId, It.IsAny<Domain.Common.Cursor>(), 10, default))
            .ReturnsAsync(mails);

        var result = await _sut.Handle(new GetUnreadPagedQuery(userId, pagination), default);

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
