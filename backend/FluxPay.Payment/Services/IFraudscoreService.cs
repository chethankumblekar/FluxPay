using FluxPay.Payment.Models;

namespace FluxPay.Payment.Services;

public interface IFraudService
{
    Task<int> EvaluateRiskAsync(CreatePaymentRequest request);
}