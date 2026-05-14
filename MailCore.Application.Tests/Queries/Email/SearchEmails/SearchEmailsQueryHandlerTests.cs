using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Emails;
using MailCore.Application.Mappers;
using MailCore.Application.Queries.Email.SearchEmails;
using MailCore.Domain.Common;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using Moq;

namespace MailCore.Application.Tests.Queries.Email.SearchEmails;

public class SearchEmailsQueryHandlerTests
{
    private readonly Mock<IEmailRepository> _emailRepo = new();
    private readonly SearchEmailsQueryHandler _sut;

    public SearchEmailsQueryHandlerTests()
    {
        _sut = new SearchEmailsQueryHandler(_emailRepo.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPagedResults()
    {
        var userId = Guid.NewGuid();
        var emails = CreateEmailList(userId, 3);

        _emailRepo.Setup(r => r.SearchPagedAsync(userId, "hello", Cursor.Initial, 20, default))
            .ReturnsAsync(emails);

        var result = await _sut.Handle(
            new SearchEmailsQuery(userId, "hello", new CursorPaginationQuery(null, 20)), default);

        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal("Hello 1", result.Items[0].Subject);
    }

    [Fact]
    public async Task Handle_NoResults_ReturnsEmptyPage()
    {
        var userId = Guid.NewGuid();

        _emailRepo.Setup(r => r.SearchPagedAsync(userId, "nonexistent", Cursor.Initial, 20, default))
            .ReturnsAsync([]);

        var result = await _sut.Handle(
            new SearchEmailsQuery(userId, "nonexistent", new CursorPaginationQuery(null, 20)), default);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Null(result.NextCursor);
    }

    [Fact]
    public async Task Handle_MoreThanPageSize_ReturnsNextCursor()
    {
        var userId = Guid.NewGuid();
        var emails = CreateEmailList(userId, 5);

        _emailRepo.Setup(r => r.SearchPagedAsync(userId, "hello", Cursor.Initial, 3, default))
            .ReturnsAsync(emails);

        var result = await _sut.Handle(
            new SearchEmailsQuery(userId, "hello", new CursorPaginationQuery(null, 3)), default);

        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count);
        Assert.NotNull(result.NextCursor);
    }

    private static List<Domain.Entities.Email> CreateEmailList(Guid userId, int count)
    {
        var emails = new List<Domain.Entities.Email>();
        for (var i = 0; i < count; i++)
        {
            emails.Add(new Domain.Entities.Email
            {
                Id = Guid.NewGuid(),
                SenderId = userId,
                Sender = new User { Email = $"sender{i}@example.com" },
                Subject = $"Hello {i + 1}",
                Body = $"Body {i + 1}",
                CreatedAt = DateTime.UtcNow.AddMinutes(-i),
                HasAttachments = false
            });
        }
        return emails;
    }
}
