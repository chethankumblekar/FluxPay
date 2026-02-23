using FluxPay.Payment.Infrastructure;
using FluxPay.Payment.Infrastructure.Security;
using FluxPay.Payment.Common.Constants;
using Microsoft.EntityFrameworkCore;

namespace FluxPay.Payment.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyMiddleware> _logger;

    public ApiKeyMiddleware(
        RequestDelegate next,
        ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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
                HeaderNames.Authorization,
                out var authHeader))
        {
            _logger.LogWarning(
                "Missing Authorization header | IP: {IP} | Path: {Path}",
                context.Connection.RemoteIpAddress,
                context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing Authorization header");
            return;
        }

        var bearerToken = authHeader.ToString();

        if (!bearerToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Invalid Authorization format | IP: {IP} | Path: {Path}",
                context.Connection.RemoteIpAddress,
                context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid Authorization format");
            return;
        }

        var rawApiKey = bearerToken["Bearer ".Length..].Trim();

        if (string.IsNullOrWhiteSpace(rawApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid API key");
            return;
        }

        var hashedKey = ApiKeyHasher.Hash(rawApiKey);

        var keyEntity = await db.ApiKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(k =>
                k.Key == hashedKey && k.IsActive);

        if (keyEntity == null)
        {
            _logger.LogWarning(
                "Invalid API key attempt | IP: {IP} | Path: {Path}",
                context.Connection.RemoteIpAddress,
                context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid API key");
            return;
        }

        context.Items["TenantId"] = keyEntity.TenantId;

        _logger.LogInformation(
            "API key validated successfully | TenantId: {TenantId}",
            keyEntity.TenantId);

        await _next(context);
    }
}