using FluxPay.Payment.Models;
using FluxPay.Payment.Services;
using Microsoft.AspNetCore.Mvc;

namespace FluxPay.Payment.Controllers;

[ApiController]
[Route("v1/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentsController(IPaymentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey,
        [FromHeader(Name = "Authorization")] string authHeader,
        [FromBody] CreatePaymentRequest request)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            return BadRequest(new { error = "Idempotency-Key required" });

        var tenantId = ExtractTenant(authHeader);

        var payment = await _service.CreateAsync(
            tenantId,
            idempotencyKey,
            request);

        return Ok(payment);
    }
    
    [HttpPost("{id}/capture")]
    public async Task<IActionResult> Capture(Guid id,
        [FromHeader(Name = "Authorization")] string authHeader)
    {
        var tenantId = ExtractTenant(authHeader);

        var payment = await _service.CaptureAsync(tenantId, id);

        return Ok(payment);
    }
    
    [HttpPost("{id}/refund")]
    public async Task<IActionResult> Refund(Guid id,
        [FromHeader(Name = "Authorization")] string authHeader)
    {
        var tenantId = ExtractTenant(authHeader);

        var payment = await _service.RefundAsync(tenantId, id);

        return Ok(payment);
    }

    private string ExtractTenant(string authHeader)
    {
        return authHeader.Replace("Bearer ", "");
    }
}