using MailCore.Application.Commands.Labels.CreateLabel;
using MailCore.Application.Commands.Labels.UpdateLabel;
using MailCore.Application.DTOs.Labels;
using MailCore.Application.Validators;

namespace MailCore.Application.Tests.Validators;

public class LabelValidatorTests
{
    private readonly CreateLabelCommandValidator _createSut = new();
    private readonly UpdateLabelCommandValidator _updateSut = new();

    // ?? CreateLabel ?????????????????????????????????????????????????????????

    [Fact]
    public void Create_Valid_Passes()
    {
     var cmd = new CreateLabelCommand(Guid.NewGuid(), new CreateLabelRequest("Work", "#FF5733"));
        Assert.True(_createSut.Validate(cmd).IsValid);
    }

    [Fact]
    public void Create_EmptyUserId_Fails()
    {
 var cmd = new CreateLabelCommand(Guid.Empty, new CreateLabelRequest("Work", null));
        Assert.False(_createSut.Validate(cmd).IsValid);
  }

    [Fact]
    public void Create_EmptyName_Fails()
    {
        var cmd = new CreateLabelCommand(Guid.NewGuid(), new CreateLabelRequest("", null));
       Assert.False(_createSut.Validate(cmd).IsValid);
    }

    [Fact]
    public void Create_NameExceeds100Chars_Fails()
    {
    var cmd = new CreateLabelCommand(Guid.NewGuid(), new CreateLabelRequest(new string('x', 101), null));
  Assert.False(_createSut.Validate(cmd).IsValid);
    }

    [Theory]
    [InlineData("red")]
    [InlineData("#GGGGGG")]
    [InlineData("#FFF")]
    [InlineData("rgb(255,0,0)")]
    public void Create_InvalidHexColor_Fails(string color)
    {
        var cmd = new CreateLabelCommand(Guid.NewGuid(), new CreateLabelRequest("Work", color));
      Assert.False(_createSut.Validate(cmd).IsValid);
  }

    [Theory]
    [InlineData("#FF5733")]
    [InlineData("#ffffff")]
    [InlineData("#000000")]
 public void Create_ValidHexColor_Passes(string color)
    {
     var cmd = new CreateLabelCommand(Guid.NewGuid(), new CreateLabelRequest("Work", color));
     Assert.True(_createSut.Validate(cmd).IsValid);
    }

  [Fact]
    public void Create_NullColor_Passes()
    {
        var cmd = new CreateLabelCommand(Guid.NewGuid(), new CreateLabelRequest("Work", null));
        Assert.True(_createSut.Validate(cmd).IsValid);
    }

    // ?? UpdateLabel ?????????????????????????????????????????????????????????

    [Fact]
    public void Update_Valid_Passes()
    {
        var cmd = new UpdateLabelCommand(Guid.NewGuid(), Guid.NewGuid(), new UpdateLabelRequest("Work", "#FF5733"));
  Assert.True(_updateSut.Validate(cmd).IsValid);
    }

    [Fact]
    public void Update_EmptyLabelId_Fails()
    {
        var cmd = new UpdateLabelCommand(Guid.NewGuid(), Guid.Empty, new UpdateLabelRequest("Work", null));
      Assert.False(_updateSut.Validate(cmd).IsValid);
    }

 [Fact]
    public void Update_EmptyName_Fails()
    {
        var cmd = new UpdateLabelCommand(Guid.NewGuid(), Guid.NewGuid(), new UpdateLabelRequest("", null));
      Assert.False(_updateSut.Validate(cmd).IsValid);
  }
}
