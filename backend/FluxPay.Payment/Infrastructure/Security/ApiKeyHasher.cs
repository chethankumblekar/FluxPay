using System.Security.Cryptography;
using System.Text;

namespace FluxPay.Payment.Infrastructure.Security;

public static class ApiKeyHasher
{
    public static string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}