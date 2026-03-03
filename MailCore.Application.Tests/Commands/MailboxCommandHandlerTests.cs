using MailCore.Application.Commands.Mailbox.MarkDeleted;
using MailCore.Application.Commands.Mailbox.MarkRead;
using MailCore.Application.Commands.Mailbox.MarkSpam;
using MailCore.Application.Commands.Mailbox.MarkStarred;
using MailCore.Application.Commands.Mailbox.MarkUnread;
using MailCore.Application.Commands.Mailbox.Restore;
using MailCore.Application.Commands.Mailbox.Unspam;
using MailCore.Application.Commands.Mailbox.Unstar;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using Moq;

namespace MailCore.Application.Tests.Commands;

public class MailboxCommandHandlerTests
{
    private readonly Mock<IMailRecipientRepository> _repo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _mailId = Guid.NewGuid();

    private MailRecipient OwnedMail() =>
        new() { Id = _mailId, UserId = _userId, IsRead = false, IsStarred = false, IsSpam = false };

    private void SetupRepo(MailRecipient? mail) =>
   _repo.Setup(r => r.GetByIdAsync(_mailId, default)).ReturnsAsync(mail);

    // ?? MarkRead ????????????????????????????????????????????????????????????

    [Fact]
    public async Task MarkRead_OwnedMail_SetsIsReadAndReturnsTrue()
    {
      var mail = OwnedMail();
        SetupRepo(mail);

       var result = await new MarkMailReadCommandHandler(_repo.Object)
   .Handle(new MarkMailReadCommand(_userId, _mailId), default);

    Assert.True(result);
 Assert.True(mail.IsRead);
    }

    [Fact]
    public async Task MarkRead_NotFound_ReturnsFalse()
{
        SetupRepo(null);
      Assert.False(await new MarkMailReadCommandHandler(_repo.Object)
       .Handle(new MarkMailReadCommand(_userId, _mailId), default));
    }

    [Fact]
    public async Task MarkRead_WrongOwner_ReturnsFalse()
    {
        SetupRepo(new MailRecipient { Id = _mailId, UserId = Guid.NewGuid() });
        Assert.False(await new MarkMailReadCommandHandler(_repo.Object)
   .Handle(new MarkMailReadCommand(_userId, _mailId), default));
    }

    // ?? MarkUnread ??????????????????????????????????????????????????????????

[Fact]
    public async Task MarkUnread_OwnedMail_SetsIsReadFalse()
    {
 var mail = OwnedMail();
   mail.IsRead = true;
      SetupRepo(mail);

     var result = await new MarkMailUnreadCommandHandler(_repo.Object)
        .Handle(new MarkMailUnreadCommand(_userId, _mailId), default);

        Assert.True(result);
        Assert.False(mail.IsRead);
    }

    // ?? MarkStarred ?????????????????????????????????????????????????????????

    [Fact]
    public async Task MarkStarred_OwnedMail_SetsIsStarredTrue()
    {
        var mail = OwnedMail();
        SetupRepo(mail);

        var result = await new MarkMailStarredCommandHandler(_repo.Object)
     .Handle(new MarkMailStarredCommand(_userId, _mailId), default);

    Assert.True(result);
        Assert.True(mail.IsStarred);
    }

    // ?? Unstar ??????????????????????????????????????????????????????????????

    [Fact]
    public async Task Unstar_OwnedMail_SetsIsStarredFalse()
    {
        var mail = OwnedMail();
        mail.IsStarred = true;
        SetupRepo(mail);

       Assert.True(await new UnstarMailCommandHandler(_repo.Object)
     .Handle(new UnstarMailCommand(_userId, _mailId), default));
        Assert.False(mail.IsStarred);
    }

    // ?? MarkSpam ?????????????????????????????????????????????????????????????

    [Fact]
    public async Task MarkSpam_OwnedMail_SetsIsSpamTrue()
    {
  var mail = OwnedMail();
     SetupRepo(mail);

        Assert.True(await new MarkMailSpamCommandHandler(_repo.Object)
   .Handle(new MarkMailSpamCommand(_userId, _mailId), default));
        Assert.True(mail.IsSpam);
    }

    // ?? Unspam ??????????????????????????????????????????????????????????????

    [Fact]
    public async Task Unspam_OwnedMail_SetsIsSpamFalse()
    {
        var mail = OwnedMail();
 mail.IsSpam = true;
        SetupRepo(mail);

     Assert.True(await new UnspamMailCommandHandler(_repo.Object)
      .Handle(new UnspamMailCommand(_userId, _mailId), default));
  Assert.False(mail.IsSpam);
    }

    // ?? MarkDeleted ??????????????????????????????????????????????????????????

    [Fact]
    public async Task MarkDeleted_OwnedMail_SetsDeletedAtAndReturnsTrue()
    {
        var mail = OwnedMail();
        SetupRepo(mail);

        var result = await new MarkMailDeletedCommandHandler(_repo.Object)
     .Handle(new MarkMailDeletedCommand(_userId, _mailId), default);

    Assert.True(result);
        Assert.NotNull(mail.DeletedAt);
  }

    [Fact]
    public async Task MarkDeleted_NotFound_ReturnsFalse()
    {
     SetupRepo(null);
        Assert.False(await new MarkMailDeletedCommandHandler(_repo.Object)
        .Handle(new MarkMailDeletedCommand(_userId, _mailId), default));
    }

    // ?? Restore ??????????????????????????????????????????????????????????????

[Fact]
    public async Task Restore_OwnedDeletedMail_ClearsDeletedAt()
    {
      var mail = OwnedMail();
  mail.DeletedAt = DateTime.UtcNow;
        SetupRepo(mail);

      Assert.True(await new RestoreMailCommandHandler(_repo.Object, _uow.Object)
            .Handle(new RestoreMailCommand(_userId, _mailId), default));
        Assert.Null(mail.DeletedAt);
    }
}
