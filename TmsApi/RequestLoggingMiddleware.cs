using System.Diagnostics;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Before Next: Instrumentation & Correlation
        var correlationId = Guid.NewGuid().ToString("N")[..8];
        context.Response.Headers["X-Correlation-Id"] = correlationId;
        
        var stopwatch = Stopwatch.StartNew();
        
        // Structured log entry
        _logger.LogInformation("Request {Method} {Path}", 
            context.Request.Method, context.Request.Path);

        // 2. Pass control to the next middleware
        await _next(context);

        // 3. After Next: Completion & Timing
        stopwatch.Stop();
        
        _logger.LogInformation("Response {StatusCode} | {Elapsed}ms | Id: {CorrelationId}", 
            context.Response.StatusCode, stopwatch.ElapsedMilliseconds, correlationId);
    }
}