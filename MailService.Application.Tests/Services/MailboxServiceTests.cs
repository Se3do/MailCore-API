using MailService.Application.Services;
using MailService.Domain.Entities;
using MailService.Domain.Enums;
using MailService.Domain.Interfaces;
using Moq;

namespace MailService.Application.Tests.Services;

public class MailboxServiceTests
{
    private static readonly DateTime FixedNow = new(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private readonly Mock<IMailRecipientRepository> _mailRecipientRepo = new();
    private readonly Mock<IEmailRepository> _emailRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly MailboxService _sut;

    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid OtherUserId = Guid.NewGuid();

    public MailboxServiceTests()
    {
        _sut = new MailboxService(_emailRepo.Object, _mailRecipientRepo.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task MarkReadAsync_OwnMail_SetsIsReadTrue()
    {
        var mr = CreateMailRecipient(UserId, isRead: false);
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mr.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mr);

        var result = await _sut.MarkReadAsync(UserId, mr.Id);

        Assert.True(result);
        Assert.True(mr.IsRead);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkReadAsync_NotFound_ReturnsFalse()
    {
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((MailRecipient?)null);

        Assert.False(await _sut.MarkReadAsync(UserId, Guid.NewGuid()));
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MarkReadAsync_OtherUsersMail_ReturnsFalse()
    {
        var mr = CreateMailRecipient(OtherUserId);
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mr.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mr);

        Assert.False(await _sut.MarkReadAsync(UserId, mr.Id));
    }

    [Fact]
    public async Task MarkUnreadAsync_OwnMail_SetsIsReadFalse()
    {
        var mr = CreateMailRecipient(UserId, isRead: true);
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mr.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mr);

        var result = await _sut.MarkUnreadAsync(UserId, mr.Id);

        Assert.True(result);
        Assert.False(mr.IsRead);
    }

    [Fact]
    public async Task StarAsync_OwnMail_SetsIsStarredTrue()
    {
        var mr = CreateMailRecipient(UserId, isStarred: false);
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mr.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mr);

        Assert.True(await _sut.StarAsync(UserId, mr.Id));
        Assert.True(mr.IsStarred);
    }

    [Fact]
    public async Task UnstarAsync_OwnMail_SetsIsStarredFalse()
    {
        var mr = CreateMailRecipient(UserId, isStarred: true);
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mr.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mr);

        Assert.True(await _sut.UnstarAsync(UserId, mr.Id));
        Assert.False(mr.IsStarred);
    }

    [Fact]
    public async Task StarAsync_OtherUsersMail_ReturnsFalse()
    {
        var mr = CreateMailRecipient(OtherUserId);
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mr.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mr);

        Assert.False(await _sut.StarAsync(UserId, mr.Id));
    }

    [Fact]
    public async Task MarkSpamAsync_OwnMail_SetsIsSpamTrue()
    {
        var mr = CreateMailRecipient(UserId, isSpam: false);
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mr.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mr);

        Assert.True(await _sut.MarkSpamAsync(UserId, mr.Id));
        Assert.True(mr.IsSpam);
    }

    [Fact]
    public async Task UnspamAsync_OwnMail_SetsIsSpamFalse()
    {
        var mr = CreateMailRecipient(UserId, isSpam: true);
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mr.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mr);

        Assert.True(await _sut.UnspamAsync(UserId, mr.Id));
        Assert.False(mr.IsSpam);
    }

    [Fact]
    public async Task DeleteAsync_OwnMail_SetsDeletedAt()
    {
        var mr = CreateMailRecipient(UserId);
        Assert.Null(mr.DeletedAt);
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mr.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mr);

        Assert.True(await _sut.DeleteAsync(UserId, mr.Id));
        Assert.NotNull(mr.DeletedAt);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NotFound_ReturnsFalse()
    {
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
       .ReturnsAsync((MailRecipient?)null);

        Assert.False(await _sut.DeleteAsync(UserId, Guid.NewGuid()));
    }

    [Fact]
    public async Task RestoreAsync_DeletedMail_ClearsDeletedAt()
    {
        var mr = CreateMailRecipient(UserId);
        mr.DeletedAt = FixedNow;
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mr.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mr);

        Assert.True(await _sut.RestoreAsync(UserId, mr.Id));
        Assert.Null(mr.DeletedAt);
    }

    [Fact]
    public async Task RestoreAsync_OtherUsersMail_ReturnsFalse()
    {
        var mr = CreateMailRecipient(OtherUserId);
        mr.DeletedAt = FixedNow;
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mr.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mr);

        Assert.False(await _sut.RestoreAsync(UserId, mr.Id));
    }

    [Fact]
    public async Task GetInboxAsync_ReturnsMappedItems()
    {
        var mrs = new List<MailRecipient> { CreateMailRecipient(UserId), CreateMailRecipient(UserId) };
        _mailRecipientRepo.Setup(r => r.GetInboxAsync(UserId, It.IsAny<CancellationToken>())).ReturnsAsync(mrs);

        var result = await _sut.GetInboxAsync(UserId);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetInboxAsync_Empty_ReturnsEmptyList()
    {
        _mailRecipientRepo.Setup(r => r.GetInboxAsync(UserId, It.IsAny<CancellationToken>()))
  .ReturnsAsync(new List<MailRecipient>());

        var result = await _sut.GetInboxAsync(UserId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSentAsync_ReturnsMappedSentEmails()
    {
        var sender = new User { Id = UserId, Name = "Alice", Email = "alice@test.com", PasswordHash = "h", CreatedAt = FixedNow };
        var recipient = new User { Id = Guid.NewGuid(), Name = "Bob", Email = "bob@test.com", PasswordHash = "h", CreatedAt = FixedNow };
        var email = new Email
        {
            Id = Guid.NewGuid(),
            SenderId = UserId,
            Sender = sender,
            Subject = "Sent",
            Body = "Body",
            CreatedAt = FixedNow,
            ThreadId = Guid.NewGuid(),
            Recipients = new List<MailRecipient>
            {
                new() { Id = Guid.NewGuid(), UserId = recipient.Id, User = recipient, Type = RecipientType.To }
            }
        };

        _emailRepo.Setup(r => r.GetSentAsync(UserId, It.IsAny<CancellationToken>()))
   .ReturnsAsync(new List<Email> { email });

        var result = await _sut.GetSentAsync(UserId);

        Assert.Single(result);
        Assert.Equal("Sent", result[0].Subject);
        Assert.Equal("alice@test.com", result[0].From);
    }

    [Fact]
    public async Task GetMailByIdAsync_OwnMail_ReturnsDetail()
    {
        var mr = CreateMailRecipient(UserId);
        mr.Labels = new List<MailRecipientLabel>();
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mr.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mr);

        var result = await _sut.GetMailByIdAsync(UserId, mr.Id);

        Assert.NotNull(result);
        Assert.Equal(mr.Id, result!.MailRecipientId);
    }

    [Fact]
    public async Task GetMailByIdAsync_NotFound_ReturnsNull()
    {
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MailRecipient?)null);

        Assert.Null(await _sut.GetMailByIdAsync(UserId, Guid.NewGuid()));
    }

    [Fact]
    public async Task GetMailByIdAsync_OtherUsersMail_ReturnsNull()
    {
        var mr = CreateMailRecipient(OtherUserId);
        mr.Labels = new List<MailRecipientLabel>();
        _mailRecipientRepo.Setup(r => r.GetByIdAsync(mr.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mr);

        Assert.Null(await _sut.GetMailByIdAsync(UserId, mr.Id));
    }

    private static MailRecipient CreateMailRecipient(
     Guid userId,
        bool isRead = false,
        bool isStarred = false,
      bool isSpam = false)
    {
        var sender = new User { Id = Guid.NewGuid(), Name = "Sender", Email = "sender@test.com", PasswordHash = "h", CreatedAt = FixedNow };
        var recipientUser = new User { Id = userId, Name = "Recipient", Email = "recipient@test.com", PasswordHash = "h", CreatedAt = FixedNow };

        var email = new Email
        {
            Id = Guid.NewGuid(),
            SenderId = sender.Id,
            Sender = sender,
            Subject = "Test Subject",
            Body = "Test Body",
            CreatedAt = FixedNow,
            ThreadId = Guid.NewGuid(),
            Recipients = new List<MailRecipient>()
        };

        var mr = new MailRecipient
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            User = recipientUser,
            EmailId = email.Id,
            Email = email,
            Type = RecipientType.To,
            IsRead = isRead,
            IsStarred = isStarred,
            IsSpam = isSpam,
            ReceivedAt = FixedNow,
            Labels = new List<MailRecipientLabel>()
        };

        email.Recipients.Add(mr);
        return mr;
    }
}
