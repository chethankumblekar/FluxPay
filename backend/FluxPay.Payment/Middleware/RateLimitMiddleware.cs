using StackExchange.Redis;

namespace FluxPay.Payment.Middleware;

public class RateLimitMiddleware(
    RequestDelegate next,
    IConnectionMultiplexer redis)
{
    private const int Limit = 10; // 10 requests
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            await next(context);
            return;
        }

        var apiKey = authHeader.ToString().Replace("Bearer ", "");
        var db = redis.GetDatabase();

        var key = $"ratelimit:{apiKey}:{DateTime.UtcNow:yyyyMMddHHmm}";

        var count = await db.StringIncrementAsync(key);

        if (count == 1)
        {
            await db.KeyExpireAsync(key, Window);
        }

        if (count > Limit)
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsync("Too Many Requests");
            return;
        }

        context.Response.Headers["X-RateLimit-Limit"] = Limit.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] =
            Math.Max(0, Limit - (int)count).ToString();

        await next(context);
    }
}