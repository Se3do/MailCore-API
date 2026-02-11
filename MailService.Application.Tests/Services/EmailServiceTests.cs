using MailService.Application.DTOs.Emails;
using MailService.Application.Services;
using MailService.Domain.Entities;
using MailService.Domain.Enums;
using MailService.Domain.Interfaces;
using Moq;

namespace MailService.Application.Tests.Services;

public class EmailServiceTests
{
    private static readonly DateTime FixedNow = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private readonly Mock<IEmailRepository> _emailRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IMailRecipientRepository> _mailRecipientRepo = new();
    private readonly Mock<IThreadRepository> _threadRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly EmailService _sut;

    private static readonly Guid SenderId = Guid.NewGuid();
    private static readonly User SenderUser = new()
    {
        Id = SenderId,
        Name = "Alice",
        Email = "alice@test.com",
        PasswordHash = "hash",
        CreatedAt = FixedNow
    };

    private static readonly Guid RecipientId = Guid.NewGuid();
    private static readonly User RecipientUser = new()
    {
        Id = RecipientId,
        Name = "Bob",
        Email = "bob@test.com",
        PasswordHash = "hash",
        CreatedAt = FixedNow
    };

    public EmailServiceTests()
    {
        _sut = new EmailService(
            _emailRepo.Object,
         _unitOfWork.Object,
    _userRepo.Object,
   _mailRecipientRepo.Object,
      _threadRepo.Object);
    }

    [Fact]
    public async Task SendAsync_WithValidRequest_CreatesEmailAndRecipients()
    {
        // Arrange
        _userRepo.Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(SenderUser);
        _userRepo.Setup(r => r.GetByEmailAsync("bob@test.com", It.IsAny<CancellationToken>()))
       .ReturnsAsync(RecipientUser);

        var request = new SendEmailRequest(
           Subject: "Hello",
                Body: "Test body",
             To: new[] { "bob@test.com" },
           Cc: null, Bcc: null, ThreadId: null, Attachments: null);

        // Act
        var result = await _sut.SendAsync(SenderId, request);

        // Assert
        Assert.Equal("Hello", result.Subject);
        Assert.Equal("Test body", result.Body);
        Assert.Equal("alice@test.com", result.From);

        _emailRepo.Verify(r => r.AddAsync(It.Is<Email>(e =>
            e.Subject == "Hello" && e.SenderId == SenderId),
            It.IsAny<CancellationToken>()), Times.Once);

        _mailRecipientRepo.Verify(r => r.AddAsync(It.Is<MailRecipient>(mr =>
           mr.UserId == RecipientId && mr.Type == RecipientType.To),
                It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithExistingThread_UsesExistingThread()
    {
        // Arrange
        var threadId = Guid.NewGuid();
        var existingThread = new Domain.Entities.Thread
        {
            Id = threadId,
            CreatedAt = FixedNow.AddHours(-1),
            LastMessageAt = FixedNow.AddHours(-1)
        };

        _userRepo.Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>()))
               .ReturnsAsync(SenderUser);
        _userRepo.Setup(r => r.GetByEmailAsync("bob@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(RecipientUser);
        _threadRepo.Setup(r => r.GetByIdAsync(threadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingThread);

        var request = new SendEmailRequest("Subject", "Body",
            To: new[] { "bob@test.com" },
        Cc: null, Bcc: null, ThreadId: threadId, Attachments: null);

        // Act
        var result = await _sut.SendAsync(SenderId, request);

        // Assert
        Assert.Equal(threadId, result.ThreadId);
    }

    [Fact]
    public async Task SendAsync_WithCcAndBcc_CreatesAllRecipientTypes()
    {
        // Arrange
        var ccUser = new User { Id = Guid.NewGuid(), Name = "Cc", Email = "cc@test.com", PasswordHash = "h", CreatedAt = FixedNow };
        var bccUser = new User { Id = Guid.NewGuid(), Name = "Bcc", Email = "bcc@test.com", PasswordHash = "h", CreatedAt = FixedNow };

        _userRepo.Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>())).ReturnsAsync(SenderUser);
        _userRepo.Setup(r => r.GetByEmailAsync("bob@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(RecipientUser);
        _userRepo.Setup(r => r.GetByEmailAsync("cc@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(ccUser);
        _userRepo.Setup(r => r.GetByEmailAsync("bcc@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(bccUser);

        var request = new SendEmailRequest("Sub", "Body",
     To: new[] { "bob@test.com" },
        Cc: new[] { "cc@test.com" },
         Bcc: new[] { "bcc@test.com" },
        ThreadId: null, Attachments: null);

        // Act
        await _sut.SendAsync(SenderId, request);

        // Assert
        _mailRecipientRepo.Verify(r => r.AddAsync(It.Is<MailRecipient>(mr => mr.Type == RecipientType.To), It.IsAny<CancellationToken>()), Times.Once);
        _mailRecipientRepo.Verify(r => r.AddAsync(It.Is<MailRecipient>(mr => mr.Type == RecipientType.Cc), It.IsAny<CancellationToken>()), Times.Once);
        _mailRecipientRepo.Verify(r => r.AddAsync(It.Is<MailRecipient>(mr => mr.Type == RecipientType.Bcc), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithNoRecipients_ThrowsArgumentException()
    {
        var request = new SendEmailRequest("Sub", "Body",
             To: new List<string>(), Cc: null, Bcc: null, ThreadId: null, Attachments: null);

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.SendAsync(SenderId, request));
    }

    [Fact]
    public async Task SendAsync_WithNullRecipients_ThrowsArgumentException()
    {
        var request = new SendEmailRequest("Sub", "Body",
        To: null!, Cc: null, Bcc: null, ThreadId: null, Attachments: null);

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.SendAsync(SenderId, request));
    }

    [Fact]
    public async Task SendAsync_SenderNotFound_ThrowsKeyNotFoundException()
    {
        _userRepo.Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>()))
 .ReturnsAsync((User?)null);

        var request = new SendEmailRequest("Sub", "Body",
              To: new[] { "bob@test.com" }, Cc: null, Bcc: null, ThreadId: null, Attachments: null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.SendAsync(SenderId, request));
    }

    [Fact]
    public async Task SendAsync_RecipientNotFound_ThrowsKeyNotFoundException()
    {
        _userRepo.Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>())).ReturnsAsync(SenderUser);
        _userRepo.Setup(r => r.GetByEmailAsync("unknown@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var request = new SendEmailRequest("Sub", "Body",
            To: new[] { "unknown@test.com" }, Cc: null, Bcc: null, ThreadId: null, Attachments: null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.SendAsync(SenderId, request));
    }

    [Fact]
    public async Task SendAsync_InvalidThreadId_ThrowsKeyNotFoundException()
    {
        _userRepo.Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>())).ReturnsAsync(SenderUser);
        _threadRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Thread?)null);

        var request = new SendEmailRequest("Sub", "Body",
     To: new[] { "bob@test.com" }, Cc: null, Bcc: null, ThreadId: Guid.NewGuid(), Attachments: null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.SendAsync(SenderId, request));
    }

    [Fact]
    public async Task ReplyAsync_WithValidRequest_PrependsRePrefix()
    {
        // Arrange
        var originalEmailId = Guid.NewGuid();
        var threadId = Guid.NewGuid();
        var originalEmail = CreateEmail(originalEmailId, threadId, "Original Subject");
        var thread = new Domain.Entities.Thread { Id = threadId, CreatedAt = FixedNow, LastMessageAt = FixedNow };

        _emailRepo.Setup(r => r.GetByIdAsync(originalEmailId, It.IsAny<CancellationToken>())).ReturnsAsync(originalEmail);
        _userRepo.Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>())).ReturnsAsync(SenderUser);
        _userRepo.Setup(r => r.GetByEmailAsync("bob@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(RecipientUser);
        _threadRepo.Setup(r => r.GetByIdAsync(threadId, It.IsAny<CancellationToken>())).ReturnsAsync(thread);

        var request = new ReplyEmailRequest("Reply body", To: new[] { "bob@test.com" }, Cc: null, Bcc: null, Attachments: null);

        // Act
        var result = await _sut.ReplyAsync(SenderId, originalEmailId, request);

        // Assert
        Assert.Equal("Re: Original Subject", result.Subject);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReplyAsync_SubjectAlreadyHasRePrefix_DoesNotDuplicate()
    {
        var originalEmailId = Guid.NewGuid();
        var threadId = Guid.NewGuid();
        var originalEmail = CreateEmail(originalEmailId, threadId, "Re: Already replied");
        var thread = new Domain.Entities.Thread { Id = threadId, CreatedAt = FixedNow, LastMessageAt = FixedNow };

        _emailRepo.Setup(r => r.GetByIdAsync(originalEmailId, It.IsAny<CancellationToken>())).ReturnsAsync(originalEmail);
        _userRepo.Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>())).ReturnsAsync(SenderUser);
        _userRepo.Setup(r => r.GetByEmailAsync("bob@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(RecipientUser);
        _threadRepo.Setup(r => r.GetByIdAsync(threadId, It.IsAny<CancellationToken>())).ReturnsAsync(thread);

        var request = new ReplyEmailRequest("Body", To: new[] { "bob@test.com" }, Cc: null, Bcc: null, Attachments: null);

        var result = await _sut.ReplyAsync(SenderId, originalEmailId, request);

        Assert.Equal("Re: Already replied", result.Subject);
    }

    [Fact]
    public async Task ReplyAsync_NoToSpecified_FallsBackToOriginalSender()
    {
        var originalEmailId = Guid.NewGuid();
        var threadId = Guid.NewGuid();
        var originalSender = new User { Id = Guid.NewGuid(), Name = "OrigSender", Email = "orig@test.com", PasswordHash = "h", CreatedAt = FixedNow };
        var originalEmail = new Email
        {
            Id = originalEmailId,
            SenderId = originalSender.Id,
            Sender = originalSender,
            Subject = "Subject",
            Body = "Body",
            CreatedAt = FixedNow,
            ThreadId = threadId
        };
        var thread = new Domain.Entities.Thread { Id = threadId, CreatedAt = FixedNow, LastMessageAt = FixedNow };

        _emailRepo.Setup(r => r.GetByIdAsync(originalEmailId, It.IsAny<CancellationToken>())).ReturnsAsync(originalEmail);
        _userRepo.Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>())).ReturnsAsync(SenderUser);
        _userRepo.Setup(r => r.GetByIdAsync(originalSender.Id, It.IsAny<CancellationToken>())).ReturnsAsync(originalSender);
        _userRepo.Setup(r => r.GetByEmailAsync("orig@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(originalSender);
        _threadRepo.Setup(r => r.GetByIdAsync(threadId, It.IsAny<CancellationToken>())).ReturnsAsync(thread);

        var request = new ReplyEmailRequest("Reply", To: null, Cc: null, Bcc: null, Attachments: null);

        var result = await _sut.ReplyAsync(SenderId, originalEmailId, request);

        _mailRecipientRepo.Verify(r => r.AddAsync(It.Is<MailRecipient>(mr =>
  mr.UserId == originalSender.Id && mr.Type == RecipientType.To),
 It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReplyAsync_OriginalEmailNotFound_ThrowsKeyNotFoundException()
    {
        _emailRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync((Email?)null);

        var request = new ReplyEmailRequest("Body", To: null, Cc: null, Bcc: null, Attachments: null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.ReplyAsync(SenderId, Guid.NewGuid(), request));
    }

    [Fact]
    public async Task ForwardAsync_WithValidRequest_PrependsFwdPrefix()
    {
        var originalEmailId = Guid.NewGuid();
        var threadId = Guid.NewGuid();
        var originalEmail = CreateEmail(originalEmailId, threadId, "Important News");

        _emailRepo.Setup(r => r.GetByIdAsync(originalEmailId, It.IsAny<CancellationToken>())).ReturnsAsync(originalEmail);
        _userRepo.Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>())).ReturnsAsync(SenderUser);
        _userRepo.Setup(r => r.GetByEmailAsync("bob@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(RecipientUser);

        var request = new ForwardEmailRequest("FYI",
     To: new[] { "bob@test.com" }, Cc: null, Bcc: null, Attachments: null);

        var result = await _sut.ForwardAsync(SenderId, originalEmailId, request);

        Assert.Equal("Fwd: Important News", result.Subject);
        Assert.NotEqual(threadId, result.ThreadId); // Forward creates a new thread
    }

    [Fact]
    public async Task ForwardAsync_SubjectAlreadyHasFwdPrefix_DoesNotDuplicate()
    {
        var originalEmailId = Guid.NewGuid();
        var originalEmail = CreateEmail(originalEmailId, Guid.NewGuid(), "Fwd: Already forwarded");

        _emailRepo.Setup(r => r.GetByIdAsync(originalEmailId, It.IsAny<CancellationToken>())).ReturnsAsync(originalEmail);
        _userRepo.Setup(r => r.GetByIdAsync(SenderId, It.IsAny<CancellationToken>())).ReturnsAsync(SenderUser);
        _userRepo.Setup(r => r.GetByEmailAsync("bob@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(RecipientUser);

        var request = new ForwardEmailRequest("FYI",
       To: new[] { "bob@test.com" }, Cc: null, Bcc: null, Attachments: null);

        var result = await _sut.ForwardAsync(SenderId, originalEmailId, request);

        Assert.Equal("Fwd: Already forwarded", result.Subject);
    }

    [Fact]
    public async Task ForwardAsync_NoRecipients_ThrowsArgumentException()
    {
        var originalEmail = CreateEmail(Guid.NewGuid(), Guid.NewGuid(), "Sub");
        _emailRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(originalEmail);

        var request = new ForwardEmailRequest("Body",
            To: new List<string>(), Cc: null, Bcc: null, Attachments: null);

        await Assert.ThrowsAsync<ArgumentException>(() =>
    _sut.ForwardAsync(SenderId, originalEmail.Id, request));
    }

    [Fact]
    public async Task ForwardAsync_OriginalNotFound_ThrowsKeyNotFoundException()
    {
        _emailRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
   .ReturnsAsync((Email?)null);

        var request = new ForwardEmailRequest("Body",
               To: new[] { "bob@test.com" }, Cc: null, Bcc: null, Attachments: null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.ForwardAsync(SenderId, Guid.NewGuid(), request));
    }

    private Email CreateEmail(Guid id, Guid threadId, string subject) => new()
    {
        Id = id,
        SenderId = SenderId,
        Sender = SenderUser,
        Subject = subject,
        Body = "Some body",
        CreatedAt = FixedNow,
        ThreadId = threadId
    };
}
