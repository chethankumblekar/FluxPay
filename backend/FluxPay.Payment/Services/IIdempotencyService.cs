namespace FluxPay.Payment.Services;

public interface IIdempotencyService
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
}