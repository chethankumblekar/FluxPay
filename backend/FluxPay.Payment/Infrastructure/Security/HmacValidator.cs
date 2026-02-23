using System.Security.Cryptography;
using System.Text;

namespace FluxPay.Payment.Infrastructure.Security;

public static class HmacValidator
{
    public static bool Validate(string secret, string payload, string signature)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var computed = Convert.ToHexString(
            hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));

        return computed.Equals(signature, StringComparison.OrdinalIgnoreCase);
    }
}