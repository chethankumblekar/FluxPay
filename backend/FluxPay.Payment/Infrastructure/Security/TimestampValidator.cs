namespace FluxPay.Payment.Infrastructure.Security;

public static class TimestampValidator
{
    public static bool IsValid(long requestTimestamp, int toleranceSeconds = 300)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return Math.Abs(now - requestTimestamp) <= toleranceSeconds;
    }
}