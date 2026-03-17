using MailCore.Application.Commands.Emails.ForwardEmail;
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

public class ForwardEmailCommandHandlerTests
{
    private readonly Mock<IEmailRepository> _emailRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IThreadRepository> _threadRepo = new();
    private readonly Mock<IMailRecipientRepository> _mailRecipientRepo = new();
    private readonly Mock<IAttachmentService> _attachmentService = new();
    private readonly EmailComposer _composer;
    private readonly ForwardEmailCommandHandler _sut;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _originalEmailId = Guid.NewGuid();
    private readonly User _sender;
    private readonly Email _originalEmail;

    public ForwardEmailCommandHandlerTests()
    {
        _sender = new User { Id = _userId, Email = "sender@example.com", Name = "Sender" };
        _originalEmail = new Email 
        { 
            Id = _originalEmailId, 
            Subject = "Original Subject", 
            Body = "Original Body",
            ThreadId = Guid.NewGuid()
        };

        _composer = new EmailComposer(
            _userRepo.Object,
            _mailRecipientRepo.Object,
            _attachmentService.Object);

        _sut = new ForwardEmailCommandHandler(
            _emailRepo.Object,
            _userRepo.Object,
            _threadRepo.Object,
            _composer);
    }

    private ForwardEmailCommand ValidCommand() => new(
        _userId,
        _originalEmailId,
        new ForwardEmailRequest(
            Body: "Forwarded Body",
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
    public async Task Handle_ValidCommand_CreatesNewThreadAndEmail()
    {
        _emailRepo.Setup(r => r.GetByIdAsync(_originalEmailId, default)).ReturnsAsync(_originalEmail);
        _userRepo.Setup(r => r.GetByIdAsync(_userId, default)).ReturnsAsync(_sender);
        SetupRecipient("recipient@example.com");

        await _sut.Handle(ValidCommand(), default);

        _threadRepo.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Thread>(), default), Times.Once);
        _emailRepo.Verify(r => r.AddAsync(It.Is<Email>(e =>
            e.SenderId == _userId &&
            e.Subject == "Fwd: Original Subject" &&
            e.Body == "Forwarded Body" &&
            e.ThreadId != _originalEmail.ThreadId), default), Times.Once);
    }

    [Fact]
    public async Task Handle_SubjectAlreadyHasFwd_DoesNotDuplicatePrefix()
    {
        var originalWithFwd = new Email { Id = _originalEmailId, Subject = "Fwd: Original Subject", ThreadId = Guid.NewGuid() };
        _emailRepo.Setup(r => r.GetByIdAsync(_originalEmailId, default)).ReturnsAsync(originalWithFwd);
        _userRepo.Setup(r => r.GetByIdAsync(_userId, default)).ReturnsAsync(_sender);
        SetupRecipient("recipient@example.com");

        var command = ValidCommand();
        await _sut.Handle(command, default);

        _emailRepo.Verify(r => r.AddAsync(It.Is<Email>(e => e.Subject == "Fwd: Original Subject"), default), Times.Once);
    }

    [Fact]
    public async Task Handle_EmailNotFound_ThrowsNotFoundException()
    {
        _emailRepo.Setup(r => r.GetByIdAsync(_originalEmailId, default)).ReturnsAsync((Email?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.Handle(ValidCommand(), default));
    }

    [Fact]
    public async Task Handle_NoRecipients_ThrowsValidationException()
    {
        _emailRepo.Setup(r => r.GetByIdAsync(_originalEmailId, default)).ReturnsAsync(_originalEmail);
        
        var command = new ForwardEmailCommand(_userId, _originalEmailId, 
            new ForwardEmailRequest("Body", Array.Empty<string>(), null, null, null));

        await Assert.ThrowsAsync<ValidationException>(
            () => _sut.Handle(command, default));
    }

    [Fact]
    public async Task Handle_SenderNotFound_ThrowsNotFoundException()
    {
        _emailRepo.Setup(r => r.GetByIdAsync(_originalEmailId, default)).ReturnsAsync(_originalEmail);
        _userRepo.Setup(r => r.GetByIdAsync(_userId, default)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.Handle(ValidCommand(), default));
    }
}
