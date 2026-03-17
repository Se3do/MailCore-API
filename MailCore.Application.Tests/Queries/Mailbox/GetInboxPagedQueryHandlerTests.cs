using MailCore.Application.Common.Pagination;
using MailCore.Application.Queries.Mailbox.GetInboxPaged;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using Moq;
using Xunit;

namespace MailCore.Application.Tests.Queries.Mailbox;

public class GetInboxPagedQueryHandlerTests
{
    private readonly Mock<IMailRecipientRepository> _mailRecipientRepo = new();
    private readonly GetInboxPagedQueryHandler _sut;

    public GetInboxPagedQueryHandlerTests()
    {
        _sut = new GetInboxPagedQueryHandler(_mailRecipientRepo.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var userId = Guid.NewGuid();
        var pagination = new CursorPaginationQuery(null, 10);
        
        var mails = new List<MailRecipient>
        {
            new MailRecipient 
            { 
                Id = Guid.NewGuid(), 
                UserId = userId, 
                ReceivedAt = DateTime.UtcNow,
                Email = new Domain.Entities.Email { Subject = "Test", CreatedAt = DateTime.UtcNow, Sender = new User { Email = "s@s.com" }, Body = "Hello" }
            }
        };

        _mailRecipientRepo.Setup(r => r.GetInboxPagedAsync(userId, It.IsAny<Domain.Common.Cursor>(), 10, default))
            .ReturnsAsync(mails);

        var result = await _sut.Handle(new GetInboxPagedQuery(userId, pagination), default);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        Assert.Single(result.Items);
    }
}
