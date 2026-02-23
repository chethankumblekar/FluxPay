namespace FluxPay.Payment.Middleware;

using Serilog;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var start = DateTime.UtcNow;

        await _next(context);

        var duration = (DateTime.UtcNow - start).TotalMilliseconds;

        Log.Information(
            "HTTP {Method} {Path} responded {StatusCode} in {Elapsed:0.0000} ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            duration);
    }
}