using MailCore.Application.Commands.Emails.ReplyEmail;
using MailCore.Application.DTOs.Emails;
using MailCore.Application.Emails;
using MailCore.Application.Exceptions;
using MailCore.Application.Interfaces.Services;
using MailCore.Application.Interfaces.Persistence;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailCore.Domain.Interfaces;
using Moq;
using Xunit;

namespace MailCore.Application.Tests.Commands;

public class ReplyEmailCommandHandlerTests
{
    private readonly Mock<IEmailRepository> _emailRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IThreadRepository> _threadRepo = new();
    private readonly Mock<IMailRecipientRepository> _mailRecipientRepo = new();
    private readonly Mock<IAttachmentService> _attachmentService = new();
    private readonly EmailComposer _composer;
    private readonly ReplyEmailCommandHandler _sut;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _originalEmailId = Guid.NewGuid();
    private readonly Guid _threadId = Guid.NewGuid();
    private readonly User _sender;
    private readonly Email _originalEmail;
    private readonly Domain.Entities.Thread _existingThread;

    public ReplyEmailCommandHandlerTests()
    {
        _sender = new User { Id = _userId, Email = "sender@example.com", Name = "Sender" };
        var originalSenderId = Guid.NewGuid();
        _originalEmail = new Email 
        { 
            Id = _originalEmailId, 
            Subject = "Original Subject", 
            Body = "Original Body",
            ThreadId = _threadId,
            SenderId = originalSenderId
        };
        _existingThread = new Domain.Entities.Thread
        {
            Id = _threadId,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            LastMessageAt = DateTime.UtcNow.AddDays(-1)
        };

        _composer = new EmailComposer(
            _userRepo.Object,
            _mailRecipientRepo.Object,
            _attachmentService.Object);

        _sut = new ReplyEmailCommandHandler(
            _emailRepo.Object,
            _userRepo.Object,
            _threadRepo.Object,
            _composer);
    }

    private ReplyEmailCommand ValidCommand() => new(
        _userId,
        _originalEmailId,
        new ReplyEmailRequest(
            Body: "Reply Body",
            To: new[] { "recipient@example.com" },
            Cc: null,
            Bcc: null,
            Attachments: null));

    private void SetupRecipient(string email)
    {
        var user = new User { Id = Guid.NewGuid(), Email = email };
        _userRepo.Setup(r => r.GetByEmailAsync(email, default)).ReturnsAsync(user);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesEmailInSameThread()
    {
        _emailRepo.Setup(r => r.GetByIdAsync(_originalEmailId, default)).ReturnsAsync(_originalEmail);
        _threadRepo.Setup(r => r.GetByIdAsync(_threadId, default)).ReturnsAsync(_existingThread);
        _userRepo.Setup(r => r.GetByIdAsync(_userId, default)).ReturnsAsync(_sender);
        SetupRecipient("recipient@example.com");

        await _sut.Handle(ValidCommand(), default);

        _emailRepo.Verify(r => r.AddAsync(It.Is<Email>(e =>
            e.SenderId == _userId &&
            e.Subject == "Re: Original Subject" &&
            e.Body == "Reply Body" &&
            e.ThreadId == _threadId), default), Times.Once);
            
        Assert.True(_existingThread.LastMessageAt > DateTime.UtcNow.AddMinutes(-1)); // verify thread time updated
    }

    [Fact]
    public async Task Handle_SubjectAlreadyHasRe_DoesNotDuplicatePrefix()
    {
        var originalWithRe = new Email { Id = _originalEmailId, Subject = "Re: Original Subject", ThreadId = _threadId };
        _emailRepo.Setup(r => r.GetByIdAsync(_originalEmailId, default)).ReturnsAsync(originalWithRe);
        _threadRepo.Setup(r => r.GetByIdAsync(_threadId, default)).ReturnsAsync(_existingThread);
        _userRepo.Setup(r => r.GetByIdAsync(_userId, default)).ReturnsAsync(_sender);
        SetupRecipient("recipient@example.com");

        var command = ValidCommand();
        await _sut.Handle(command, default);

        _emailRepo.Verify(r => r.AddAsync(It.Is<Email>(e => e.Subject == "Re: Original Subject"), default), Times.Once);
    }

    [Fact]
    public async Task Handle_NoToGiven_UsesOriginalSenderAsDefault()
    {
        _emailRepo.Setup(r => r.GetByIdAsync(_originalEmailId, default)).ReturnsAsync(_originalEmail);
        _threadRepo.Setup(r => r.GetByIdAsync(_threadId, default)).ReturnsAsync(_existingThread);
        
        var originalSender = new User { Id = _originalEmail.SenderId, Email = "originalsender@example.com" };
        _userRepo.Setup(r => r.GetByIdAsync(_originalEmail.SenderId, default)).ReturnsAsync(originalSender);
        _userRepo.Setup(r => r.GetByEmailAsync("originalsender@example.com", default)).ReturnsAsync(originalSender);

        var command = new ReplyEmailCommand(_userId, _originalEmailId, 
            new ReplyEmailRequest("Reply Body", null, null, null, null));

        await _sut.Handle(command, default);

        _emailRepo.Verify(r => r.AddAsync(It.IsAny<Email>(), default), Times.Once);
        // MailRecipient for To would be checked via composer
        _mailRecipientRepo.Verify(r => r.AddAsync(It.Is<MailRecipient>(mr => mr.Type == RecipientType.To), default), Times.Once);
    }

    [Fact]
    public async Task Handle_EmailNotFound_ThrowsNotFoundException()
    {
        _emailRepo.Setup(r => r.GetByIdAsync(_originalEmailId, default)).ReturnsAsync((Email?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.Handle(ValidCommand(), default));
    }

    [Fact]
    public async Task Handle_ThreadNotFound_ThrowsNotFoundException()
    {
        _emailRepo.Setup(r => r.GetByIdAsync(_originalEmailId, default)).ReturnsAsync(_originalEmail);
        _threadRepo.Setup(r => r.GetByIdAsync(_threadId, default)).ReturnsAsync((Domain.Entities.Thread?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.Handle(ValidCommand(), default));
    }
}
