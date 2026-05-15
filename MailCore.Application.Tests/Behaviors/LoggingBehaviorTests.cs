using MailCore.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace MailCore.Application.Tests.Behaviors;

public class LoggingBehaviorTests
{
    public record TestRequest(string Value) : IRequest<string>;

    private static Task<string> SuccessNext() => Task.FromResult("ok");
    private static Task<string> FailureNext() => throw new InvalidOperationException("fail");

    [Fact]
    public async Task Handle_SuccessfulRequest_LogsStartAndEnd()
    {
        var loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, string>>>();
        var behavior = new LoggingBehavior<TestRequest, string>(loggerMock.Object);

        var result = await behavior.Handle(new TestRequest("x"), _ => SuccessNext(), default);

        Assert.Equal("ok", result);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handled")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_FailingRequest_LogsStartAndFailureAndPropagates()
    {
        var loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, string>>>();
        var behavior = new LoggingBehavior<TestRequest, string>(loggerMock.Object);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => behavior.Handle(new TestRequest("x"), _ => FailureNext(), default));

        Assert.Equal("fail", ex.Message);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
