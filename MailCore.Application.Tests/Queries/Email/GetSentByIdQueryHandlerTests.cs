using MailCore.Application.Exceptions;
using MailCore.Application.Queries.Email.GetSentById;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using Moq;
using Xunit;

namespace MailCore.Application.Tests.Queries.Email;

public class GetSentByIdQueryHandlerTests
{
    private readonly Mock<IEmailRepository> _emailRepo = new();
    private readonly GetSentByIdQueryHandler _sut;

    public GetSentByIdQueryHandlerTests()
    {
        _sut = new GetSentByIdQueryHandler(_emailRepo.Object);
    }

    [Fact]
    public async Task Handle_EmailExistsAndBelongsToSender_ReturnsDto()
    {
        var emailId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var email = new Domain.Entities.Email { Id = emailId, SenderId = userId, Subject = "Hello", Body = "World", CreatedAt = DateTime.UtcNow };
        
        _emailRepo.Setup(r => r.GetByIdAsync(emailId, default)).ReturnsAsync(email);

        var result = await _sut.Handle(new GetSentByIdQuery(userId, emailId), default);

        Assert.NotNull(result);
        Assert.Equal("Hello", result.Subject);
    }

    [Fact]
    public async Task Handle_EmailNotFound_ThrowsNotFoundException()
    {
        var emailId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _emailRepo.Setup(r => r.GetByIdAsync(emailId, default)).ReturnsAsync((Domain.Entities.Email?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.Handle(new GetSentByIdQuery(userId, emailId), default));
    }

    [Fact]
    public async Task Handle_EmailBelongsToAnotherSender_ThrowsForbiddenException()
    {
        var emailId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var email = new Domain.Entities.Email { Id = emailId, SenderId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        
        _emailRepo.Setup(r => r.GetByIdAsync(emailId, default)).ReturnsAsync(email);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _sut.Handle(new GetSentByIdQuery(userId, emailId), default));
    }
}
