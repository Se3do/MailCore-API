using MailService.Application.DTOs.Drafts;
using MailService.Application.Services;
using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using Moq;

namespace MailService.Application.Tests.Services;

public class DraftServiceTests
{
    private static readonly DateTime FixedNow = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private readonly Mock<IDraftRepository> _draftRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly DraftService _sut;

    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid OtherUserId = Guid.NewGuid();

    public DraftServiceTests()
    {
        _sut = new DraftService(_unitOfWork.Object, _draftRepo.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsDraftDto()
    {
        var request = new CreateDraftRequest("Draft Subject", "Draft Body", ThreadId: null);

        var result = await _sut.CreateAsync(UserId, request);

        Assert.Equal("Draft Subject", result.Subject);
        Assert.Equal("Draft Body", result.Body);
        Assert.Null(result.ThreadId);
        Assert.NotEqual(Guid.Empty, result.Id);

        _draftRepo.Verify(r => r.AddAsync(It.Is<Draft>(d =>
            d.UserId == UserId && d.Subject == "Draft Subject"),
        It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithThreadId_AssociatesThread()
    {
        var threadId = Guid.NewGuid();
        var request = new CreateDraftRequest("Sub", "Body", ThreadId: threadId);

        var result = await _sut.CreateAsync(UserId, request);

        Assert.Equal(threadId, result.ThreadId);
    }

    [Fact]
    public async Task UpdateAsync_OwnDraft_UpdatesAndReturnsDto()
    {
        var draftId = Guid.NewGuid();
        var existingDraft = new Draft
        {
            Id = draftId,
            UserId = UserId,
            Subject = "Old",
            Body = "Old body",
            UpdatedAt = FixedNow.AddHours(-1)
        };

        _draftRepo.Setup(r => r.GetByIdAsync(draftId, It.IsAny<CancellationToken>()))
       .ReturnsAsync(existingDraft);

        var request = new UpdateDraftRequest("New Subject", "New Body");

        var result = await _sut.UpdateAsync(UserId, draftId, request);

        Assert.Equal("New Subject", result.Subject);
        Assert.Equal("New Body", result.Body);
        _draftRepo.Verify(r => r.UpdateAsync(draftId, It.IsAny<Draft>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_DraftNotFound_ThrowsKeyNotFoundException()
    {
        _draftRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
 .ReturnsAsync((Draft?)null);

        var request = new UpdateDraftRequest("Sub", "Body");

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.UpdateAsync(UserId, Guid.NewGuid(), request));
    }

    [Fact]
    public async Task UpdateAsync_DraftBelongsToOtherUser_ThrowsKeyNotFoundException()
    {
        var draftId = Guid.NewGuid();
        var otherDraft = new Draft { Id = draftId, UserId = OtherUserId, Subject = "S", Body = "B", UpdatedAt = FixedNow };

        _draftRepo.Setup(r => r.GetByIdAsync(draftId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherDraft);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
_sut.UpdateAsync(UserId, draftId, new UpdateDraftRequest("S", "B")));
    }

    [Fact]
    public async Task DeleteAsync_OwnDraft_ReturnsTrue()
    {
        var draftId = Guid.NewGuid();
        _draftRepo.Setup(r => r.GetByIdAsync(draftId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Draft { Id = draftId, UserId = UserId, Subject = "S", Body = "B", UpdatedAt = FixedNow });

        var result = await _sut.DeleteAsync(UserId, draftId);

        Assert.True(result);
        _draftRepo.Verify(r => r.DeleteAsync(draftId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DraftNotFound_ReturnsFalse()
    {
        _draftRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Draft?)null);

        var result = await _sut.DeleteAsync(UserId, Guid.NewGuid());

        Assert.False(result);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_DraftBelongsToOtherUser_ReturnsFalse()
    {
        var draftId = Guid.NewGuid();
        _draftRepo.Setup(r => r.GetByIdAsync(draftId, It.IsAny<CancellationToken>()))
          .ReturnsAsync(new Draft { Id = draftId, UserId = OtherUserId, Subject = "S", Body = "B", UpdatedAt = FixedNow });

        var result = await _sut.DeleteAsync(UserId, draftId);

        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDrafts()
    {
        var drafts = new List<Draft>
        {
            new() { Id = Guid.NewGuid(), UserId = UserId, Subject = "D1", Body = "B1", UpdatedAt = FixedNow },
            new() { Id = Guid.NewGuid(), UserId = UserId, Subject = "D2", Body = "B2", UpdatedAt = FixedNow.AddMinutes(1) }
      };

        _draftRepo.Setup(r => r.GetAllAsync(UserId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(drafts);

        var result = await _sut.GetAllAsync(UserId);

        Assert.Equal(2, result.Count);
        Assert.Equal("D1", result[0].Subject);
        Assert.Equal("D2", result[1].Subject);
    }

    [Fact]
    public async Task GetAllAsync_NoDrafts_ReturnsEmptyList()
    {
        _draftRepo.Setup(r => r.GetAllAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Draft>());

        var result = await _sut.GetAllAsync(UserId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_OwnDraft_ReturnsDto()
    {
        var draftId = Guid.NewGuid();
        _draftRepo.Setup(r => r.GetByIdAsync(draftId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(new Draft { Id = draftId, UserId = UserId, Subject = "S", Body = "B", UpdatedAt = FixedNow });

        var result = await _sut.GetByIdAsync(UserId, draftId);

        Assert.NotNull(result);
        Assert.Equal(draftId, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_DraftNotFound_ReturnsNull()
    {
        _draftRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Draft?)null);

        var result = await _sut.GetByIdAsync(UserId, Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_DraftBelongsToOtherUser_ReturnsNull()
    {
        var draftId = Guid.NewGuid();
        _draftRepo.Setup(r => r.GetByIdAsync(draftId, It.IsAny<CancellationToken>()))
      .ReturnsAsync(new Draft { Id = draftId, UserId = OtherUserId, Subject = "S", Body = "B", UpdatedAt = FixedNow });

        var result = await _sut.GetByIdAsync(UserId, draftId);

        Assert.Null(result);
    }
}
