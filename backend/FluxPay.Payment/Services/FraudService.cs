using FluxPay.Payment.Models;

namespace FluxPay.Payment.Services;

public class FraudService : IFraudService
{
    public Task<int> EvaluateRiskAsync(CreatePaymentRequest request) // deterministic logic for now
    { 
        var risk = 0;

        if (request.Amount > 100000)
            risk += 50;

        if (request.Currency != "INR")
            risk += 20;

        if (request.CustomerId.StartsWith("test"))
            risk += 10;

        return Task.FromResult(risk);
    }
}