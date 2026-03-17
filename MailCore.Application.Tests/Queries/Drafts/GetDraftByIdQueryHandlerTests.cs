using MailCore.Application.Queries.Drafts.GetDraftById;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using Moq;
using Xunit;

namespace MailCore.Application.Tests.Queries.Drafts;

public class GetDraftByIdQueryHandlerTests
{
    private readonly Mock<IDraftRepository> _draftRepo = new();
    private readonly GetDraftByIdQueryHandler _sut;

    public GetDraftByIdQueryHandlerTests()
    {
        _sut = new GetDraftByIdQueryHandler(_draftRepo.Object);
    }

    [Fact]
    public async Task Handle_DraftExistsAndBelongsToUser_ReturnsDto()
    {
        var draftId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var draft = new Draft { Id = draftId, UserId = userId, Subject = "Subject", Body = "Body", UpdatedAt = DateTime.UtcNow };
        
        _draftRepo.Setup(r => r.GetByIdAsync(draftId, default)).ReturnsAsync(draft);

        var result = await _sut.Handle(new GetDraftByIdQuery(userId, draftId), default);

        Assert.NotNull(result);
        Assert.Equal("Subject", result.Subject);
    }

    [Fact]
    public async Task Handle_DraftNotFound_ReturnsNull()
    {
        var draftId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _draftRepo.Setup(r => r.GetByIdAsync(draftId, default)).ReturnsAsync((Draft?)null);

        var result = await _sut.Handle(new GetDraftByIdQuery(userId, draftId), default);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task Handle_DraftBelongsToAnotherUser_ReturnsNull()
    {
        var draftId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var draft = new Draft { Id = draftId, UserId = anotherUserId, Subject = "Subject", UpdatedAt = DateTime.UtcNow };
        
        _draftRepo.Setup(r => r.GetByIdAsync(draftId, default)).ReturnsAsync(draft);

        var result = await _sut.Handle(new GetDraftByIdQuery(userId, draftId), default);

        Assert.Null(result);
    }
}
