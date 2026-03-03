using MailCore.Application.Commands.Emails.SendEmail;
using MailCore.Application.DTOs.Emails;
using MailCore.Application.Validators;

namespace MailCore.Application.Tests.Validators;

public class SendEmailCommandValidatorTests
{
    private readonly SendEmailCommandValidator _sut = new();

    private static SendEmailCommand Valid() => new(
      Guid.NewGuid(),
    new SendEmailRequest(
    Subject: "Hello",
    Body: "World",
    To: ["alice@example.com"],
     Cc: null,
      Bcc: null,
      ThreadId: null,
          Attachments: null));

  [Fact]
    public void Valid_Command_Passes()
    {
     var result = _sut.Validate(Valid());
        Assert.True(result.IsValid);
  }

    [Fact]
 public void EmptyUserId_Fails()
    {
 var cmd = Valid() with { UserId = Guid.Empty };
        var result = _sut.Validate(cmd);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("UserId"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
  public void EmptySubject_Fails(string? subject)
    {
 var req = Valid().Request with { Subject = subject! };
  var result = _sut.Validate(Valid() with { Request = req });
        Assert.False(result.IsValid);
    }

    [Fact]
    public void SubjectExceeds500Chars_Fails()
    {
        var req = Valid().Request with { Subject = new string('x', 501) };
        Assert.False(_sut.Validate(Valid() with { Request = req }).IsValid);
    }

    [Fact]
    public void EmptyBody_Fails()
    {
        var req = Valid().Request with { Body = "" };
        Assert.False(_sut.Validate(Valid() with { Request = req }).IsValid);
    }

    [Fact]
    public void EmptyToList_Fails()
    {
  var req = Valid().Request with { To = [] };
  Assert.False(_sut.Validate(Valid() with { Request = req }).IsValid);
    }

    [Fact]
    public void InvalidEmailInTo_Fails()
    {
    var req = Valid().Request with { To = ["not-an-email"] };
     Assert.False(_sut.Validate(Valid() with { Request = req }).IsValid);
  }

    [Fact]
    public void InvalidEmailInCc_Fails()
    {
  var req = Valid().Request with { Cc = ["not-an-email"] };
      Assert.False(_sut.Validate(Valid() with { Request = req }).IsValid);
    }

    [Fact]
    public void InvalidEmailInBcc_Fails()
    {
        var req = Valid().Request with { Bcc = ["not-an-email"] };
 Assert.False(_sut.Validate(Valid() with { Request = req }).IsValid);
  }

 [Fact]
    public void ValidCcAndBcc_Pass()
    {
      var req = Valid().Request with
        {
       Cc = ["cc@example.com"],
   Bcc = ["bcc@example.com"]
        };
 Assert.True(_sut.Validate(Valid() with { Request = req }).IsValid);
    }
}
