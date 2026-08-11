using MailCore.Application.Exceptions;
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
        var draft = Draft.Create(userId, "Subject", "Body", id: draftId);

        _draftRepo.Setup(r => r.GetByIdAsync(draftId, default)).ReturnsAsync(draft);

        var result = await _sut.Handle(new GetDraftByIdQuery(userId, draftId), default);

        Assert.Equal("Subject", result.Subject);
    }

    [Fact]
    public async Task Handle_DraftNotFound_ThrowsNotFound()
    {
        var draftId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _draftRepo.Setup(r => r.GetByIdAsync(draftId, default)).ReturnsAsync((Draft?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.Handle(new GetDraftByIdQuery(userId, draftId), default));
    }

    [Fact]
    public async Task Handle_DraftBelongsToAnotherUser_ThrowsForbidden()
    {
        var draftId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var draft = Draft.Create(anotherUserId, "Subject", "", id: draftId);

        _draftRepo.Setup(r => r.GetByIdAsync(draftId, default)).ReturnsAsync(draft);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _sut.Handle(new GetDraftByIdQuery(userId, draftId), default));
    }
}
