using System.Text.Json;
using FluxPay.Payment.Domain.Exceptions;

namespace FluxPay.Payment.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString();

        try
        {
            context.Response.Headers.Append("X-Correlation-Id", correlationId);
            await _next(context);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error");

            context.Response.StatusCode = ex switch
            {
                NotFoundException => 404,
                InvalidStateException => 400,
                _ => 400
            };

            await WriteErrorResponse(context, ex.Message,
                ex.ErrorCode,
                correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception while processing {Path}",
                context.Request.Path);

            context.Response.StatusCode = 500;

            await WriteErrorResponse(context,
                "An unexpected error occurred",
                "internal_error",
                correlationId);
        }
    }

    private static async Task WriteErrorResponse(
        HttpContext context,
        string message,
        string code,
        string correlationId)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = new
            {
                type = "api_error",
                message,
                code,
                correlation_id = correlationId
            }
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}