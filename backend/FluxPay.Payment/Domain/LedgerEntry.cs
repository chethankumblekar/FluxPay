namespace FluxPay.Payment.Domain;

public class LedgerEntry
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public string TenantId { get; set; } = default!;
    public string AccountType { get; set; } = default!; // PLATFORM / MERCHANT
    public string EntryType { get; set; } = default!;   // DEBIT / CREDIT
    public long Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}