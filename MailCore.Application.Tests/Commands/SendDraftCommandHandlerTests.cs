using MailCore.Application.Commands.Drafts.SendDraft;
using MailCore.Application.Common.Drafts;
using MailCore.Application.Emails;
using MailCore.Application.Exceptions;
using MailCore.Application.Interfaces.Services;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using Moq;

namespace MailCore.Application.Tests.Commands;

public class SendDraftCommandHandlerTests
{
    private readonly Mock<IDraftRepository> _draftRepo = new();
    private readonly Mock<IEmailRepository> _emailRepo = new();
    private readonly Mock<IThreadRepository> _threadRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IMailRecipientRepository> _mailRecipientRepo = new();
    private readonly Mock<IAttachmentService> _attachmentService = new();
    private readonly EmailComposer _composer;
    private readonly SendDraftCommandHandler _sut;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _draftId = Guid.NewGuid();
    private readonly Guid _threadId = Guid.NewGuid();

    public SendDraftCommandHandlerTests()
    {
        _composer = new EmailComposer(
            _userRepo.Object,
            _mailRecipientRepo.Object,
            _attachmentService.Object);

        _sut = new SendDraftCommandHandler(
            _draftRepo.Object,
            _emailRepo.Object,
            _threadRepo.Object,
            _userRepo.Object,
            _composer);
    }

    private Draft ValidDraft() => Draft.Create(
        _userId, "Subject", "Body",
        to: DraftRecipientsCodec.Serialize(["recipient@example.com"]),
        threadId: null, id: _draftId);

    private Draft ValidDraftWithEmptySubject() => Draft.Create(
        _userId, "", "Body",
        to: DraftRecipientsCodec.Serialize(["recipient@example.com"]),
        threadId: null, id: _draftId);

    private Draft DraftWithThread() => Draft.Create(
        _userId, "Subject", "Body",
        to: DraftRecipientsCodec.Serialize(["recipient@example.com"]),
        threadId: _threadId, id: _draftId);

    private Draft DraftWithCcBcc() => Draft.Create(
        _userId, "Subject", "Body",
        to: DraftRecipientsCodec.Serialize(["to@example.com"]),
        cc: DraftRecipientsCodec.Serialize(["cc@example.com"]),
        bcc: DraftRecipientsCodec.Serialize(["bcc@example.com"]),
        threadId: null, id: _draftId);

    private void SetupSender()
    {
        _userRepo.Setup(r => r.GetByIdAsync(_userId, default))
            .ReturnsAsync(new User { Id = _userId, Email = "sender@example.com", Name = "Sender" });
    }

    private void SetupRecipient(string email = "recipient@example.com")
    {
        _userRepo.Setup(r => r.GetByEmailAsync(email, default))
            .ReturnsAsync(new User { Id = Guid.NewGuid(), Email = email });
    }

    [Fact]
    public async Task Handle_ValidDraftWithoutThread_CreatesNewThreadAndEmailAndDeletesDraft()
    {
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync(ValidDraft());
        SetupSender();
        SetupRecipient();

        await _sut.Handle(new SendDraftCommand(_userId, _draftId), default);

        _threadRepo.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Thread>(), default), Times.Once);
        _emailRepo.Verify(r => r.AddAsync(It.Is<Email>(e =>
            e.SenderId == _userId &&
            e.Subject == "Subject" &&
            e.Body == "Body"), default), Times.Once);
        _draftRepo.Verify(r => r.DeleteAsync(_draftId, default), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidDraftWithExistingThread_UsesExistingThread()
    {
        var thread = Domain.Entities.Thread.Create(lastMessageAt: DateTime.UtcNow.AddHours(-1), id: _threadId);
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync(DraftWithThread());
        _threadRepo.Setup(r => r.GetByIdAsync(_threadId, default)).ReturnsAsync(thread);
        SetupSender();
        SetupRecipient();

        await _sut.Handle(new SendDraftCommand(_userId, _draftId), default);

        _threadRepo.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Thread>(), default), Times.Never);
        _emailRepo.Verify(r => r.AddAsync(It.Is<Email>(e => e.ThreadId == _threadId), default), Times.Once);
        _draftRepo.Verify(r => r.DeleteAsync(_draftId, default), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptySubject_DefaultsToNoSubject()
    {
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync(ValidDraftWithEmptySubject());
        SetupSender();
        SetupRecipient();

        await _sut.Handle(new SendDraftCommand(_userId, _draftId), default);

        _emailRepo.Verify(r => r.AddAsync(It.Is<Email>(e =>
            e.Subject == "(No subject)"), default), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCcAndBcc_CallsAddRecipientsForEach()
    {
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync(DraftWithCcBcc());
        SetupSender();
        SetupRecipient("to@example.com");
        SetupRecipient("cc@example.com");
        SetupRecipient("bcc@example.com");

        await _sut.Handle(new SendDraftCommand(_userId, _draftId), default);

        _mailRecipientRepo.Verify(r => r.AddAsync(It.Is<MailRecipient>(mr =>
            mr.Type == Domain.Enums.RecipientType.To), default), Times.Once);
        _mailRecipientRepo.Verify(r => r.AddAsync(It.Is<MailRecipient>(mr =>
            mr.Type == Domain.Enums.RecipientType.Cc), default), Times.Once);
        _mailRecipientRepo.Verify(r => r.AddAsync(It.Is<MailRecipient>(mr =>
            mr.Type == Domain.Enums.RecipientType.Bcc), default), Times.Once);
        _draftRepo.Verify(r => r.DeleteAsync(_draftId, default), Times.Once);
    }

    [Fact]
    public async Task Handle_DraftNotFound_ThrowsNotFoundException()
    {
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync((Draft?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.Handle(new SendDraftCommand(_userId, _draftId), default));

        _emailRepo.Verify(r => r.AddAsync(It.IsAny<Email>(), default), Times.Never);
        _draftRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), default), Times.Never);
    }

    [Fact]
    public async Task Handle_WrongOwner_ThrowsForbiddenException()
    {
        var otherUserId = Guid.NewGuid();
        var draft = Draft.Create(otherUserId, "Subject", "Body", id: _draftId);
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync(draft);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _sut.Handle(new SendDraftCommand(_userId, _draftId), default));

        _emailRepo.Verify(r => r.AddAsync(It.IsAny<Email>(), default), Times.Never);
        _draftRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), default), Times.Never);
    }

    [Fact]
    public async Task Handle_EmptyBody_ThrowsValidationException()
    {
        var draft = Draft.Create(_userId, "Subject", "", id: _draftId);
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync(draft);

        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _sut.Handle(new SendDraftCommand(_userId, _draftId), default));

        Assert.Contains("body cannot be empty", ex.Message, StringComparison.OrdinalIgnoreCase);
        _emailRepo.Verify(r => r.AddAsync(It.IsAny<Email>(), default), Times.Never);
        _draftRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), default), Times.Never);
    }

    [Fact]
    public async Task Handle_NullBody_ThrowsValidationException()
    {
        var draft = Draft.Create(_userId, "Subject", null!, id: _draftId);
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync(draft);

        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _sut.Handle(new SendDraftCommand(_userId, _draftId), default));

        Assert.Contains("body cannot be empty", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_NoToRecipients_ThrowsValidationException()
    {
        var draft = Draft.Create(_userId, "Subject", "Body", to: null, id: _draftId);
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync(draft);

        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _sut.Handle(new SendDraftCommand(_userId, _draftId), default));

        Assert.Contains("at least one recipient", ex.Message, StringComparison.OrdinalIgnoreCase);
        _emailRepo.Verify(r => r.AddAsync(It.IsAny<Email>(), default), Times.Never);
    }

    [Fact]
    public async Task Handle_SenderNotFound_ThrowsNotFoundException()
    {
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync(ValidDraft());
        _userRepo.Setup(r => r.GetByIdAsync(_userId, default)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.Handle(new SendDraftCommand(_userId, _draftId), default));

        _emailRepo.Verify(r => r.AddAsync(It.IsAny<Email>(), default), Times.Never);
        _draftRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), default), Times.Never);
    }

    [Fact]
    public async Task Handle_ThreadNotFound_ThrowsNotFoundException()
    {
        _draftRepo.Setup(r => r.GetByIdAsync(_draftId, default)).ReturnsAsync(DraftWithThread());
        _threadRepo.Setup(r => r.GetByIdAsync(_threadId, default)).ReturnsAsync((Domain.Entities.Thread?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.Handle(new SendDraftCommand(_userId, _draftId), default));

        _emailRepo.Verify(r => r.AddAsync(It.IsAny<Email>(), default), Times.Never);
        _draftRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), default), Times.Never);
    }
}
