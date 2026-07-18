using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace TmsApi.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        var correlationId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
        var stopwatch = Stopwatch.StartNew();

        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestName"] = requestName,
            ["CorrelationId"] = correlationId
        });

        logger.LogInformation("Handling {RequestName} (cid={CorrelationId})", requestName, correlationId);

        try
        {
            var response = await next();
            stopwatch.Stop();

            logger.LogInformation("Handled {RequestName} in {ElapsedMs}ms (cid={CorrelationId})", 
                requestName, stopwatch.ElapsedMilliseconds, correlationId);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "Failed {RequestName} after {ElapsedMs}ms (cid={CorrelationId})", 
                requestName, stopwatch.ElapsedMilliseconds, correlationId);
            throw;
        }
    }
}