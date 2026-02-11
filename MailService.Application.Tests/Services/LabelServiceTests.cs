using MailService.Application.DTOs.Labels;
using MailService.Application.Services;
using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using Moq;

namespace MailService.Application.Tests.Services;

public class LabelServiceTests
{
    private readonly Mock<ILabelRepository> _labelRepo = new();
    private readonly Mock<IMailRecipientRepository> _mailRecipientRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly LabelService _sut;

    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid OtherUserId = Guid.NewGuid();

    public LabelServiceTests()
    {
        _sut = new LabelService(_labelRepo.Object, _unitOfWork.Object, _mailRecipientRepo.Object);
    }

    [Fact]
    public async Task CreateAsync_WithColor_ReturnsLabelDto()
    {
        var request = new CreateLabelRequest("Urgent", "#ff0000");

        var result = await _sut.CreateAsync(UserId, request);

        Assert.Equal("Urgent", result.Name);
        Assert.Equal("#ff0000", result.Color);
        Assert.NotEqual(Guid.Empty, result.Id);

        _labelRepo.Verify(r => r.AddAsync(It.Is<Label>(l =>
       l.UserId == UserId && l.Name == "Urgent"),
       It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_NullColor_DefaultsToEmptyString()
    {
        var request = new CreateLabelRequest("Work", null);

        var result = await _sut.CreateAsync(UserId, request);

        Assert.Equal(string.Empty, result.Color);
    }

    [Fact]
    public async Task UpdateAsync_OwnLabel_UpdatesAndReturnsDto()
    {
        var labelId = Guid.NewGuid();
        var existing = new Label { Id = labelId, UserId = UserId, Name = "Old", Color = "blue" };
        _labelRepo.Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var result = await _sut.UpdateAsync(UserId, labelId, new UpdateLabelRequest("New", "#green"));

        Assert.Equal("New", result.Name);
        Assert.Equal("#green", result.Color);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_LabelNotFound_ThrowsKeyNotFoundException()
    {
        _labelRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((Label?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.UpdateAsync(UserId, Guid.NewGuid(), new UpdateLabelRequest("N", "C")));
    }

    [Fact]
    public async Task UpdateAsync_LabelBelongsToOtherUser_ThrowsKeyNotFoundException()
    {
        var labelId = Guid.NewGuid();
        _labelRepo.Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
          .ReturnsAsync(new Label { Id = labelId, UserId = OtherUserId, Name = "X", Color = "Y" });

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        _sut.UpdateAsync(UserId, labelId, new UpdateLabelRequest("N", "C")));
    }

    [Fact]
    public async Task DeleteAsync_OwnLabel_ReturnsTrue()
    {
        var labelId = Guid.NewGuid();
        _labelRepo.Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
   .ReturnsAsync(new Label { Id = labelId, UserId = UserId, Name = "L", Color = "C" });

        var result = await _sut.DeleteAsync(UserId, labelId);

        Assert.True(result);
        _labelRepo.Verify(r => r.DeleteAsync(labelId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        _labelRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Label?)null);

        Assert.False(await _sut.DeleteAsync(UserId, Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteAsync_OtherUsersLabel_ReturnsFalse()
    {
        var labelId = Guid.NewGuid();
        _labelRepo.Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
  .ReturnsAsync(new Label { Id = labelId, UserId = OtherUserId, Name = "L", Color = "C" });

        Assert.False(await _sut.DeleteAsync(UserId, labelId));
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUserLabels()
    {
        _labelRepo.Setup(r => r.GetAllAsync(UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Label>
      {
  new() { Id = Guid.NewGuid(), UserId = UserId, Name = "A", Color = "red" },
     new() { Id = Guid.NewGuid(), UserId = UserId, Name = "B", Color = "blue" }
});

        var result = await _sut.GetAllAsync(UserId);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task AssignLabelAsync_ValidOwnership_AddsLabelToMail()
    {
        var labelId = Guid.NewGuid();
        var mailId = Guid.NewGuid();
        var label = new Label { Id = labelId, UserId = UserId, Name = "L", Color = "C" };
        var mailRecipient = new MailRecipient
        {
            Id = mailId,
            UserId = UserId,
            Labels = new List<MailRecipientLabel>()
        };

        _labelRepo.Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>())).ReturnsAsync(label);
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mailId, It.IsAny<CancellationToken>())).ReturnsAsync(mailRecipient);

        var result = await _sut.AssignLabelAsync(UserId, mailId, labelId);

        Assert.True(result);
        Assert.Single(mailRecipient.Labels);
        Assert.Equal(labelId, mailRecipient.Labels.First().LabelId);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AssignLabelAsync_AlreadyAssigned_ReturnsTrueWithoutDuplicate()
    {
        var labelId = Guid.NewGuid();
        var mailId = Guid.NewGuid();
        var label = new Label { Id = labelId, UserId = UserId, Name = "L", Color = "C" };
        var mailRecipient = new MailRecipient
        {
            Id = mailId,
            UserId = UserId,
            Labels = new List<MailRecipientLabel>
            {
       new() { MailRecipientId = mailId, LabelId = labelId }
       }
        };

        _labelRepo.Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>())).ReturnsAsync(label);
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mailId, It.IsAny<CancellationToken>())).ReturnsAsync(mailRecipient);

        var result = await _sut.AssignLabelAsync(UserId, mailId, labelId);

        Assert.True(result);
        Assert.Single(mailRecipient.Labels); // no duplicate
    }

    [Fact]
    public async Task AssignLabelAsync_LabelNotOwned_ReturnsFalse()
    {
        var labelId = Guid.NewGuid();
        _labelRepo.Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
  .ReturnsAsync(new Label { Id = labelId, UserId = OtherUserId, Name = "L", Color = "C" });

        Assert.False(await _sut.AssignLabelAsync(UserId, Guid.NewGuid(), labelId));
    }

    [Fact]
    public async Task AssignLabelAsync_MailNotOwned_ReturnsFalse()
    {
        var labelId = Guid.NewGuid();
        var mailId = Guid.NewGuid();
        _labelRepo.Setup(r => r.GetByIdAsync(labelId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(new Label { Id = labelId, UserId = UserId, Name = "L", Color = "C" });
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mailId, It.IsAny<CancellationToken>()))
     .ReturnsAsync(new MailRecipient { Id = mailId, UserId = OtherUserId, Labels = new List<MailRecipientLabel>() });

        Assert.False(await _sut.AssignLabelAsync(UserId, mailId, labelId));
    }

    [Fact]
    public async Task UnassignLabelAsync_ExistingLink_RemovesLabel()
    {
        var labelId = Guid.NewGuid();
        var mailId = Guid.NewGuid();
        var link = new MailRecipientLabel { MailRecipientId = mailId, LabelId = labelId };
        var mailRecipient = new MailRecipient
        {
            Id = mailId,
            UserId = UserId,
            Labels = new List<MailRecipientLabel> { link }
        };

        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mailId, It.IsAny<CancellationToken>())).ReturnsAsync(mailRecipient);

        var result = await _sut.UnassignLabelAsync(UserId, mailId, labelId);

        Assert.True(result);
        Assert.Empty(mailRecipient.Labels);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UnassignLabelAsync_LinkNotPresent_ReturnsTrueIdempotent()
    {
        var mailId = Guid.NewGuid();
        var mailRecipient = new MailRecipient
        {
            Id = mailId,
            UserId = UserId,
            Labels = new List<MailRecipientLabel>()
        };
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mailId, It.IsAny<CancellationToken>())).ReturnsAsync(mailRecipient);

        var result = await _sut.UnassignLabelAsync(UserId, mailId, Guid.NewGuid());

        Assert.True(result);
    }

    [Fact]
    public async Task UnassignLabelAsync_MailNotOwned_ReturnsFalse()
    {
        var mailId = Guid.NewGuid();
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mailId, It.IsAny<CancellationToken>()))
          .ReturnsAsync(new MailRecipient { Id = mailId, UserId = OtherUserId, Labels = new List<MailRecipientLabel>() });

        Assert.False(await _sut.UnassignLabelAsync(UserId, mailId, Guid.NewGuid()));
    }
}
