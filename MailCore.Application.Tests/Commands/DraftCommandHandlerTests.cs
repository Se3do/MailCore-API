using MailCore.Application.Commands.Drafts.CreateDraft;
using MailCore.Application.Commands.Drafts.DeleteDraft;
using MailCore.Application.Commands.Drafts.UpdateDraft;
using MailCore.Application.DTOs.Drafts;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using Moq;

namespace MailCore.Application.Tests.Commands;

public class DraftCommandHandlerTests
{
    private readonly Mock<IDraftRepository> _draftRepo = new();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _draftId = Guid.NewGuid();

    // ?? CreateDraft ?????????????????????????????????????????????????????????

    [Fact]
    public async Task CreateDraft_ReturnsNewDraftId()
    {
        var handler = new CreateDraftCommandHandler(_draftRepo.Object);
        var cmd = new CreateDraftCommand(_userId, new CreateDraftRequest("Subject", "Body", null));

        var id = await handler.Handle(cmd, default);

        Assert.NotEqual(Guid.Empty, id);
        _draftRepo.Verify(r => r.AddAsync(It.Is<Draft>(d =>
    d.UserId == _userId &&
    d.Subject == "Subject" &&
      d.Body == "Body"), default), Times.Once);
    }

    [Fact]
    public async Task CreateDraft_SetsUpdatedAt()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var handler = new CreateDraftCommandHandler(_draftRepo.Object);

        await handler.Handle(
            new CreateDraftCommand(_userId, new CreateDraftRequest("S", "B", null)), default);

        _draftRepo.Verify(r => r.AddAsync(
            It.Is<Draft>(d => d.UpdatedAt >= before), default), Times.Once);
    }

    // ?? UpdateDraft ?????????????????????????????????????????????????????????

    [Fact]
    public async Task UpdateDraft_ExistingOwnedDraft_UpdatesAndReturnsTrue()
    {
        var draft = new Draft { Id = _draftId, UserId = _userId, Subject = "Old", Body = "Old body" };
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync(draft);

        var handler = new UpdateDraftCommandHandler(_draftRepo.Object);
        var cmd = new UpdateDraftCommand(_userId, _draftId, new UpdateDraftRequest("New", "New body"));

        var result = await handler.Handle(cmd, default);

        Assert.True(result);
        Assert.Equal("New", draft.Subject);
        Assert.Equal("New body", draft.Body);
        _draftRepo.Verify(r => r.UpdateAsync(_draftId, draft, default), Times.Once);
    }

    [Fact]
    public async Task UpdateDraft_NotFound_ReturnsFalse()
    {
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync((Draft?)null);

        var result = await new UpdateDraftCommandHandler(_draftRepo.Object)
            .Handle(new UpdateDraftCommand(_userId, _draftId, new UpdateDraftRequest("X", "Y")), default);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateDraft_WrongOwner_ReturnsFalse()
    {
        var draft = new Draft { Id = _draftId, UserId = Guid.NewGuid() };
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync(draft);

        Assert.False(await new UpdateDraftCommandHandler(_draftRepo.Object)
 .Handle(new UpdateDraftCommand(_userId, _draftId, new UpdateDraftRequest("X", "Y")), default));
    }

    // ?? DeleteDraft ?????????????????????????????????????????????????????????

    [Fact]
    public async Task DeleteDraft_ExistingOwnedDraft_DeletesAndReturnsTrue()
    {
   var draft = new Draft { Id = _draftId, UserId = _userId };
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync(draft);

        var result = await new DeleteDraftCommandHandler(_draftRepo.Object)
            .Handle(new DeleteDraftCommand(_userId, _draftId), default);

        Assert.True(result);
  _draftRepo.Verify(r => r.DeleteAsync(_draftId, default), Times.Once);
    }

    [Fact]
    public async Task DeleteDraft_NotFound_ReturnsFalse()
    {
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync((Draft?)null);

        Assert.False(await new DeleteDraftCommandHandler(_draftRepo.Object)
          .Handle(new DeleteDraftCommand(_userId, _draftId), default));

        _draftRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), default), Times.Never);
    }

    [Fact]
    public async Task DeleteDraft_WrongOwner_ReturnsFalse()
    {
        var draft = new Draft { Id = _draftId, UserId = Guid.NewGuid() };
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync(draft);

        Assert.False(await new DeleteDraftCommandHandler(_draftRepo.Object)
        .Handle(new DeleteDraftCommand(_userId, _draftId), default));

        _draftRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), default), Times.Never);
    }
}
