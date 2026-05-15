using MailCore.Application.Exceptions;
using MailCore.Application.Queries.Email.GetDeliveryStatus;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailCore.Domain.Interfaces;
using Moq;

namespace MailCore.Application.Tests.Queries.Email.GetDeliveryStatus;

public class GetDeliveryStatusQueryHandlerTests
{
    private readonly Mock<IEmailRepository> _emailRepo = new();
    private readonly GetDeliveryStatusQueryHandler _sut;

    public GetDeliveryStatusQueryHandlerTests()
    {
        _sut = new GetDeliveryStatusQueryHandler(_emailRepo.Object);
    }

    [Fact]
    public async Task Handle_EmailExistsAndBelongsToSender_ReturnsDto()
    {
        var emailId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var email = Domain.Entities.Email.Create(userId, "", "", id: emailId);
        email.MarkAsFailed("Connection timeout");
        email.MarkAsFailed("Connection timeout");

        _emailRepo.Setup(r => r.GetByIdAsync(emailId, default)).ReturnsAsync(email);

        var result = await _sut.Handle(new GetDeliveryStatusQuery(userId, emailId), default);

        Assert.NotNull(result);
        Assert.Equal(emailId, result.Id);
        Assert.Equal("Pending", result!.Status);
        Assert.Null(result.SentAt);
        Assert.Equal(2, result.SendAttempts);
        Assert.Equal("Connection timeout", result.LastSendError);
    }

    [Fact]
    public async Task Handle_EmailNotFound_ThrowsNotFoundException()
    {
        var emailId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _emailRepo.Setup(r => r.GetByIdAsync(emailId, default)).ReturnsAsync((Domain.Entities.Email?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.Handle(new GetDeliveryStatusQuery(userId, emailId), default));
    }

    [Fact]
    public async Task Handle_EmailBelongsToAnotherSender_ThrowsForbiddenException()
    {
        var emailId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var email = Domain.Entities.Email.Create(Guid.NewGuid(), "", "", id: emailId);

        _emailRepo.Setup(r => r.GetByIdAsync(emailId, default)).ReturnsAsync(email);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _sut.Handle(new GetDeliveryStatusQuery(userId, emailId), default));
    }

    [Fact]
    public async Task Handle_SentEmail_ReturnsSentStatus()
    {
        var emailId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sentAt = DateTime.UtcNow;
        var email = Domain.Entities.Email.Create(userId, "", "", id: emailId);
        email.MarkAsSent(sentAt);
        SetField(email, nameof(Domain.Entities.Email.SendAttempts), 1);

        _emailRepo.Setup(r => r.GetByIdAsync(emailId, default)).ReturnsAsync(email);

        var result = await _sut.Handle(new GetDeliveryStatusQuery(userId, emailId), default);

        Assert.Equal("Sent", result!.Status);
        Assert.Equal(sentAt, result.SentAt);
        Assert.Equal(1, result.SendAttempts);
        Assert.Null(result.LastSendError);
    }

    private static void SetField<T>(T target, string propertyName, object value)
    {
        var field = typeof(T).GetField($"<{propertyName}>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field!.SetValue(target, value);
    }
}
