namespace FluxPay.Payment.Domain;

public class Payment
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = default!;
    public long Amount { get; set; }
    public string Currency { get; set; } = default!;
    public string Status { get; set; } = PaymentStatus.Created;
    public string IdempotencyKey { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int RiskScore { get; set; }
}
