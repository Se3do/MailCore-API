using MailCore.API.Hubs;
using MailCore.API.Notifications;
using MailCore.Application.DTOs.Emails;
using MailCore.Application.Notifications;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace MailCore.Application.Tests.Notifications;

public class EmailSentNotificationHandlerTests
{
    private readonly Mock<IHubContext<MailHub>> _hubContext = new();
    private readonly Mock<IHubClients> _hubClients = new();
    private readonly Mock<IClientProxy> _clientProxy = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly EmailSentNotificationHandler _sut;

    public EmailSentNotificationHandlerTests()
    {
        _hubContext.Setup(h => h.Clients).Returns(_hubClients.Object);
        _sut = new EmailSentNotificationHandler(_hubContext.Object, _userRepo.Object);
    }

    [Fact]
    public async Task Handle_SendsNotificationToEachRecipientGroup()
    {
        var recipient1 = Guid.NewGuid();
        var recipient2 = Guid.NewGuid();

        _userRepo.Setup(r => r.GetByEmailAsync("alice@example.com", default))
            .ReturnsAsync(new User { Id = recipient1, Email = "alice@example.com" });
        _userRepo.Setup(r => r.GetByEmailAsync("bob@example.com", default))
            .ReturnsAsync(new User { Id = recipient2, Email = "bob@example.com" });
        _hubClients.Setup(c => c.Group(recipient1.ToString())).Returns(_clientProxy.Object);
        _hubClients.Setup(c => c.Group(recipient2.ToString())).Returns(_clientProxy.Object);

        var notification = new EmailSentNotification(
            "sender@example.com",
            ["alice@example.com", "bob@example.com"],
            new EmailSummaryDto(Guid.NewGuid(), "Hello", "Hi there", "sender@example.com",
                DateTimeOffset.UtcNow, null, false));

        await _sut.Handle(notification, default);

        _hubClients.Verify(c => c.Group(recipient1.ToString()), Times.Once);
        _hubClients.Verify(c => c.Group(recipient2.ToString()), Times.Once);
        _clientProxy.Verify(c => c.SendCoreAsync("NewEmail", It.IsAny<object?[]>(), default), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_SkipsRecipientsWithNoUserAccount()
    {
        _userRepo.Setup(r => r.GetByEmailAsync("exists@example.com", default))
            .ReturnsAsync(new User { Id = Guid.NewGuid(), Email = "exists@example.com" });
        _userRepo.Setup(r => r.GetByEmailAsync("missing@example.com", default))
            .ReturnsAsync((User?)null);
        _hubClients.Setup(c => c.Group(It.IsAny<string>())).Returns(_clientProxy.Object);

        var notification = new EmailSentNotification(
            "sender@example.com",
            ["exists@example.com", "missing@example.com"],
            new EmailSummaryDto(Guid.NewGuid(), "Test", "Body", "sender@example.com",
                DateTimeOffset.UtcNow, null, false));

        await _sut.Handle(notification, default);

        _clientProxy.Verify(c => c.SendCoreAsync("NewEmail", It.IsAny<object?[]>(), default), Times.Once);
    }

    [Fact]
    public async Task Handle_NoRecipients_DoesNotSend()
    {
        var notification = new EmailSentNotification(
            "sender@example.com",
            [],
            new EmailSummaryDto(Guid.NewGuid(), "Test", "Body", "sender@example.com",
                DateTimeOffset.UtcNow, null, false));

        await _sut.Handle(notification, default);

        _clientProxy.Verify(c => c.SendCoreAsync("NewEmail", It.IsAny<object?[]>(), default), Times.Never);
    }
}
