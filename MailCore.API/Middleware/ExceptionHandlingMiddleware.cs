using MailCore.Application.Exceptions;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, ex.Message);
            await WriteKnownError(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Unexpected error"
            });
        }
    }

    private static Task WriteKnownError(HttpContext ctx, AppException ex)
    {
        ctx.Response.StatusCode = ex switch
        {
            NotFoundException => 404,
            ValidationException => 400,
            ForbiddenException => 403,
            _ => 400
        };

        return ctx.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
}