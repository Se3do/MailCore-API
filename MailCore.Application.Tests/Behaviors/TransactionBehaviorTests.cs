using MailCore.Application.Common.Behaviors;
using MailCore.Domain.Common;
using MailCore.Domain.Interfaces;
using MediatR;
using Moq;

namespace MailCore.Application.Tests.Behaviors;

public class TransactionBehaviorTests
{
    private record TestCommand(string Value) : IRequest<string>, ICommand;

    private static Task<string> SuccessNext() => Task.FromResult("ok");
    private static Task<string> FailureNext() => throw new InvalidOperationException("fail");

    [Fact]
    public async Task Handle_SuccessfulRequest_CallsSaveChangesOnce()
    {
        var uowMock = new Mock<IUnitOfWork>();
        var behavior = new TransactionBehavior<TestCommand, string>(uowMock.Object);

        var result = await behavior.Handle(new TestCommand("x"), _ => SuccessNext(), default);

        Assert.Equal("ok", result);
        uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FailingRequest_DoesNotCallSaveChanges()
    {
        var uowMock = new Mock<IUnitOfWork>();
        var behavior = new TransactionBehavior<TestCommand, string>(uowMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => behavior.Handle(new TestCommand("x"), _ => FailureNext(), default));

        uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
