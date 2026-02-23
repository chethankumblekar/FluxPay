namespace FluxPay.Payment.Models;

public class CreatePaymentRequest
{
    public long Amount { get; set; }
    public string Currency { get; set; } = default!;
    public string CustomerId { get; set; } = default!;
    public string Description { get; set; } = default!;
}