using MailCore.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace MailCore.API.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException => (404, exception.Message),
            ValidationException => (400, exception.Message),
            ForbiddenException => (403, exception.Message),
            KeyNotFoundException => (404, exception.Message),
            ArgumentException => (400, exception.Message),
            _ => (500, "An unexpected error occurred.")
        };

        _logger.Log(exception is AppException ? LogLevel.Warning : LogLevel.Error, exception, message);

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new { error = message }, cancellationToken);
        return true;
    }
}
