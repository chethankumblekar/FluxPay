namespace FluxPay.Payment.Domain;

public static class PaymentStatus
{
    public const string Created = "CREATED";
    public const string Authorized = "AUTHORIZED";
    public const string Captured = "CAPTURED";
    public const string Failed = "FAILED";
    public const string Refunded = "REFUNDED";
}