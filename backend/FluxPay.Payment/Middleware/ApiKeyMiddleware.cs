using FluxPay.Payment.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FluxPay.Payment.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        PaymentDbContext db)
    {
        
        // Skip health endpoints
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }
        
        if (!context.Request.Headers.TryGetValue(
                "Authorization",
                out var authHeader))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Missing Authorization header");
            return;
        }

        var apiKey = authHeader.ToString()
            .Replace("Bearer ", "");

        var keyEntity = await db.ApiKeys
            .FirstOrDefaultAsync(k =>
                k.Key == apiKey && k.IsActive);

        if (keyEntity == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API key");
            return;
        }

        context.Items["TenantId"] = keyEntity.TenantId;

        await _next(context);
    }
}