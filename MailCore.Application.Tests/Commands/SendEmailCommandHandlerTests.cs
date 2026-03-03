using MailCore.Application.Commands.Emails.SendEmail;
using MailCore.Application.DTOs.Emails;
using MailCore.Application.Emails;
using MailCore.Application.Interfaces.Services;
using MailCore.Application.Interfaces.Persistence;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailCore.Domain.Interfaces;
using Moq;

namespace MailCore.Application.Tests.Commands;

public class SendEmailCommandHandlerTests
{
    private readonly Mock<IEmailRepository> _emailRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
  private readonly Mock<IThreadRepository> _threadRepo = new();
    private readonly Mock<IMailRecipientRepository> _mailRecipientRepo = new();
    private readonly Mock<IAttachmentService> _attachmentService = new();
    private readonly EmailComposer _composer;
    private readonly SendEmailCommandHandler _sut;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly User _sender;

    public SendEmailCommandHandlerTests()
    {
        _sender = new User { Id = _userId, Email = "sender@example.com", Name = "Sender" };

      _composer = new EmailComposer(
       _userRepo.Object,
   _mailRecipientRepo.Object,
 _attachmentService.Object);

        _sut = new SendEmailCommandHandler(
         _emailRepo.Object,
            _userRepo.Object,
          _threadRepo.Object,
            _composer);
    }

    private SendEmailCommand ValidCommand(Guid? threadId = null) => new(
_userId,
        new SendEmailRequest(
   Subject: "Hello",
            Body: "World",
         To: ["recipient@example.com"],
            Cc: null,
            Bcc: null,
       ThreadId: threadId,
       Attachments: null));

    private void SetupRecipient(string email)
    {
        var user = new User { Id = Guid.NewGuid(), Email = email };
_userRepo.Setup(r => r.GetByEmailAsync(email, default)).ReturnsAsync(user);
 }

    // ?? Happy paths ?????????????????????????????????????????????????????????

    [Fact]
    public async Task Handle_ValidCommand_CreatesNewThread()
    {
      _userRepo.Setup(r => r.GetByIdAsync(_userId, default)).ReturnsAsync(_sender);
  SetupRecipient("recipient@example.com");

        await _sut.Handle(ValidCommand(), default);

        _threadRepo.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Thread>(), default), Times.Once);
        _emailRepo.Verify(r => r.AddAsync(It.Is<Email>(e =>
   e.SenderId == _userId &&
            e.Subject == "Hello" &&
            e.Body == "World"), default), Times.Once);
 }

    [Fact]
    public async Task Handle_ExistingThreadId_UsesExistingThread()
    {
        var threadId = Guid.NewGuid();
        var thread = new Domain.Entities.Thread { Id = threadId, LastMessageAt = DateTime.UtcNow.AddHours(-1) };

        _userRepo.Setup(r => r.GetByIdAsync(_userId, default)).ReturnsAsync(_sender);
        _threadRepo.Setup(r => r.GetByIdAsync(threadId, default)).ReturnsAsync(thread);
        SetupRecipient("recipient@example.com");

        await _sut.Handle(ValidCommand(threadId), default);

        _threadRepo.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Thread>(), default), Times.Never);
 _emailRepo.Verify(r => r.AddAsync(It.Is<Email>(e => e.ThreadId == threadId), default), Times.Once);
    }

    [Fact]
public async Task Handle_AddsMailRecipientForEachToAddress()
    {
   _userRepo.Setup(r => r.GetByIdAsync(_userId, default)).ReturnsAsync(_sender);
  SetupRecipient("recipient@example.com");

        await _sut.Handle(ValidCommand(), default);

        _mailRecipientRepo.Verify(r => r.AddAsync(It.Is<MailRecipient>(mr =>
       mr.Type == RecipientType.To), default), Times.Once);
    }

    // ?? Failure paths ????????????????????????????????????????????????????????

    [Fact]
    public async Task Handle_SenderNotFound_ThrowsKeyNotFoundException()
    {
  _userRepo.Setup(r => r.GetByIdAsync(_userId, default)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
        () => _sut.Handle(ValidCommand(), default));

    _emailRepo.Verify(r => r.AddAsync(It.IsAny<Email>(), default), Times.Never);
    }

    [Fact]
    public async Task Handle_ThreadNotFound_ThrowsKeyNotFoundException()
{
   var threadId = Guid.NewGuid();
        _userRepo.Setup(r => r.GetByIdAsync(_userId, default)).ReturnsAsync(_sender);
_threadRepo.Setup(r => r.GetByIdAsync(threadId, default)).ReturnsAsync((Domain.Entities.Thread?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
() => _sut.Handle(ValidCommand(threadId), default));
    }

    [Fact]
    public async Task Handle_RecipientNotFound_ThrowsKeyNotFoundException()
{
        _userRepo.Setup(r => r.GetByIdAsync(_userId, default)).ReturnsAsync(_sender);
   _userRepo.Setup(r => r.GetByEmailAsync("recipient@example.com", default))
   .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.Handle(ValidCommand(), default));
    }
}
