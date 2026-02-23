namespace FluxPay.Payment.Domain;

public class ApiKey
{
    public Guid Id { get; set; }
    public string Key { get; set; } = null!;
    public string TenantId { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}