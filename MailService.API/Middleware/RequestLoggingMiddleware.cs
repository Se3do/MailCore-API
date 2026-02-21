using System.Diagnostics;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        await _next(context);

        sw.Stop();

        _logger.LogInformation(
            "HTTP {Method} {Path} → {StatusCode} ({Elapsed} ms)",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            sw.ElapsedMilliseconds);
    }
}