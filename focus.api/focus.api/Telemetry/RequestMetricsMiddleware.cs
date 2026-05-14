using System.Diagnostics;
using Focus.Infrastructure.Telemetry;

namespace Focus.Api.Telemetry;

public class RequestMetricsMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IClickHouseTelemetrySink sink)
    {
        var sw = Stopwatch.StartNew();
        await next(context);
        sw.Stop();

        var route = context.Request.Path.Value ?? "/";
        sink.EnqueueMetric("http_request_duration_ms", sw.Elapsed.TotalMilliseconds, route, context.Response.StatusCode, sw.Elapsed.TotalMilliseconds);
    }
}
