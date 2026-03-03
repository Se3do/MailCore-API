using MailCore.Application.Commands.Labels.AssignLabel;
using MailCore.Application.Commands.Labels.CreateLabel;
using MailCore.Application.Commands.Labels.DeleteLabel;
using MailCore.Application.Commands.Labels.UnassignLabel;
using MailCore.Application.Commands.Labels.UpdateLabel;
using MailCore.Application.DTOs.Labels;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using Moq;

namespace MailCore.Application.Tests.Commands;

public class LabelCommandHandlerTests
{
    private readonly Mock<ILabelRepository> _labelRepo = new();
    private readonly Mock<IMailRecipientRepository> _mailRecipientRepo = new();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _labelId = Guid.NewGuid();
    private readonly Guid _mailId = Guid.NewGuid();

    // ?? CreateLabel ?????????????????????????????????????????????????????????

    [Fact]
    public async Task CreateLabel_ReturnsNewLabelId()
    {
        var handler = new CreateLabelCommandHandler(_labelRepo.Object);
        var cmd = new CreateLabelCommand(_userId, new CreateLabelRequest("Work", "#FF5733"));

        var id = await handler.Handle(cmd, default);

        Assert.NotEqual(Guid.Empty, id);
        _labelRepo.Verify(r => r.AddAsync(It.Is<Label>(l =>
            l.Name == "Work" &&
  l.Color == "#FF5733" &&
   l.UserId == _userId), default), Times.Once);
    }

    [Fact]
    public async Task CreateLabel_NullColor_DefaultsToEmptyString()
    {
        await new CreateLabelCommandHandler(_labelRepo.Object)
 .Handle(new CreateLabelCommand(_userId, new CreateLabelRequest("Work", null)), default);

     _labelRepo.Verify(r => r.AddAsync(
          It.Is<Label>(l => l.Color == string.Empty), default), Times.Once);
    }

    // ?? UpdateLabel ?????????????????????????????????????????????????????????

    [Fact]
    public async Task UpdateLabel_OwnedLabel_UpdatesAndReturnsTrue()
    {
        var label = new Label { Id = _labelId, UserId = _userId, Name = "Old", Color = "blue" };
        _labelRepo.Setup(r => r.GetByIdAsync(_labelId, default)).ReturnsAsync(label);

        var result = await new UpdateLabelCommandHandler(_labelRepo.Object)
            .Handle(new UpdateLabelCommand(_userId, _labelId, new UpdateLabelRequest("New", "#000000")), default);

   Assert.True(result);
Assert.Equal("New", label.Name);
        Assert.Equal("#000000", label.Color);
    _labelRepo.Verify(r => r.UpdateAsync(_labelId, label, default), Times.Once);
    }

  [Fact]
    public async Task UpdateLabel_NotFound_ReturnsFalse()
    {
     _labelRepo.Setup(r => r.GetByIdAsync(_labelId, default)).ReturnsAsync((Label?)null);

     Assert.False(await new UpdateLabelCommandHandler(_labelRepo.Object)
  .Handle(new UpdateLabelCommand(_userId, _labelId, new UpdateLabelRequest("X", null)), default));
    }

    [Fact]
    public async Task UpdateLabel_WrongOwner_ReturnsFalse()
    {
   var label = new Label { Id = _labelId, UserId = Guid.NewGuid() };
        _labelRepo.Setup(r => r.GetByIdAsync(_labelId, default)).ReturnsAsync(label);

       Assert.False(await new UpdateLabelCommandHandler(_labelRepo.Object)
  .Handle(new UpdateLabelCommand(_userId, _labelId, new UpdateLabelRequest("X", null)), default));
    }

    // ?? DeleteLabel ?????????????????????????????????????????????????????????

    [Fact]
    public async Task DeleteLabel_OwnedLabel_DeletesAndReturnsTrue()
    {
 var label = new Label { Id = _labelId, UserId = _userId };
        _labelRepo.Setup(r => r.GetByIdAsync(_labelId, default)).ReturnsAsync(label);

    var result = await new DeleteLabelCommandHandler(_labelRepo.Object)
   .Handle(new DeleteLabelCommand(_userId, _labelId), default);

      Assert.True(result);
    _labelRepo.Verify(r => r.DeleteAsync(_labelId, default), Times.Once);
    }

    [Fact]
    public async Task DeleteLabel_NotFound_ReturnsFalse()
    {
        _labelRepo.Setup(r => r.GetByIdAsync(_labelId, default)).ReturnsAsync((Label?)null);

        Assert.False(await new DeleteLabelCommandHandler(_labelRepo.Object)
            .Handle(new DeleteLabelCommand(_userId, _labelId), default));

     _labelRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), default), Times.Never);
  }

    [Fact]
  public async Task DeleteLabel_WrongOwner_ReturnsFalse()
    {
     var label = new Label { Id = _labelId, UserId = Guid.NewGuid() };
    _labelRepo.Setup(r => r.GetByIdAsync(_labelId, default)).ReturnsAsync(label);

  Assert.False(await new DeleteLabelCommandHandler(_labelRepo.Object)
            .Handle(new DeleteLabelCommand(_userId, _labelId), default));

        _labelRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), default), Times.Never);
    }

    // ?? AssignLabel ?????????????????????????????????????????????????????????

    [Fact]
    public async Task AssignLabel_ValidOwnership_AddsLabelAndReturnsTrue()
    {
   var label = new Label { Id = _labelId, UserId = _userId };
        var mr = new MailRecipient { Id = _mailId, UserId = _userId, Labels = [] };

        _labelRepo.Setup(r => r.GetByIdAsync(_labelId, default)).ReturnsAsync(label);
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(_mailId, default)).ReturnsAsync(mr);

        var result = await new AssignLabelCommandHandler(_labelRepo.Object, _mailRecipientRepo.Object)
 .Handle(new AssignLabelCommand(_userId, _mailId, _labelId), default);

        Assert.True(result);
        Assert.Single(mr.Labels);
    }

    [Fact]
    public async Task AssignLabel_AlreadyAssigned_ReturnsTrueWithoutDuplicate()
  {
        var label = new Label { Id = _labelId, UserId = _userId };
 var mr = new MailRecipient
        {
        Id = _mailId, UserId = _userId,
            Labels = [new MailRecipientLabel { LabelId = _labelId }]
        };

        _labelRepo.Setup(r => r.GetByIdAsync(_labelId, default)).ReturnsAsync(label);
      _mailRecipientRepo.Setup(r => r.GetByIdAsync(_mailId, default)).ReturnsAsync(mr);

  Assert.True(await new AssignLabelCommandHandler(_labelRepo.Object, _mailRecipientRepo.Object)
       .Handle(new AssignLabelCommand(_userId, _mailId, _labelId), default));
        Assert.Single(mr.Labels);
    }

    [Fact]
    public async Task AssignLabel_LabelNotOwned_ReturnsFalse()
    {
        _labelRepo.Setup(r => r.GetByIdAsync(_labelId, default))
            .ReturnsAsync(new Label { Id = _labelId, UserId = Guid.NewGuid() });

      Assert.False(await new AssignLabelCommandHandler(_labelRepo.Object, _mailRecipientRepo.Object)
       .Handle(new AssignLabelCommand(_userId, _mailId, _labelId), default));
 }

    [Fact]
    public async Task AssignLabel_MailNotOwned_ReturnsFalse()
    {
        _labelRepo.Setup(r => r.GetByIdAsync(_labelId, default))
    .ReturnsAsync(new Label { Id = _labelId, UserId = _userId });
 _mailRecipientRepo.Setup(r => r.GetByIdAsync(_mailId, default))
    .ReturnsAsync(new MailRecipient { Id = _mailId, UserId = Guid.NewGuid() });

        Assert.False(await new AssignLabelCommandHandler(_labelRepo.Object, _mailRecipientRepo.Object)
   .Handle(new AssignLabelCommand(_userId, _mailId, _labelId), default));
    }

    // ?? UnassignLabel ????????????????????????????????????????????????????????

    [Fact]
    public async Task UnassignLabel_AssignedLabel_RemovesAndReturnsTrue()
    {
    var mr = new MailRecipient
        {
            Id = _mailId, UserId = _userId,
            Labels = [new MailRecipientLabel { LabelId = _labelId, MailRecipientId = _mailId }]
        };
      _mailRecipientRepo.Setup(r => r.GetByIdAsync(_mailId, default)).ReturnsAsync(mr);

        var result = await new UnassignLabelCommandHandler(_mailRecipientRepo.Object)
   .Handle(new UnassignLabelCommand(_userId, _mailId, _labelId), default);

        Assert.True(result);
 Assert.Empty(mr.Labels);
    }

    [Fact]
    public async Task UnassignLabel_LabelNotAssigned_ReturnsTrueWithoutChange()
    {
    var mr = new MailRecipient { Id = _mailId, UserId = _userId, Labels = [] };
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(_mailId, default)).ReturnsAsync(mr);

        Assert.True(await new UnassignLabelCommandHandler(_mailRecipientRepo.Object)
            .Handle(new UnassignLabelCommand(_userId, _mailId, _labelId), default));
    }

    [Fact]
    public async Task UnassignLabel_MailNotOwned_ReturnsFalse()
  {
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(_mailId, default))
    .ReturnsAsync(new MailRecipient { Id = _mailId, UserId = Guid.NewGuid() });

        Assert.False(await new UnassignLabelCommandHandler(_mailRecipientRepo.Object)
          .Handle(new UnassignLabelCommand(_userId, _mailId, _labelId), default));
    }
}
