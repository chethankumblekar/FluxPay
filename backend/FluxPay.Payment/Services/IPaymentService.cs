using FluxPay.Payment.Models;

namespace FluxPay.Payment.Services;

public interface IPaymentService
{
    Task<Domain.Payment> CreateAsync(
        string tenantId,
        string idempotencyKey,
        CreatePaymentRequest request);
    Task<Domain.Payment> CaptureAsync(string tenantId, Guid paymentId);
    Task<Domain.Payment> RefundAsync(string tenantId, Guid paymentId);
}
