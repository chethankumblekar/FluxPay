namespace FluxPay.Payment.Common.Constants;

public static class HeaderNames
{
    public const string CorrelationId = "X-Correlation-Id";
    public const string IdempotencyKey = "Idempotency-Key";
    public const string Signature = "X-Signature";
    public const string Timestamp = "X-Timestamp";
    public const string Authorization = "Authorization";
}