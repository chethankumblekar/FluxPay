using StackExchange.Redis;

namespace FluxPay.Payment.Services;

public class IdempotencyService(IConnectionMultiplexer redis) : IIdempotencyService
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<string?> GetAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.IsNullOrEmpty ? null : value.ToString();
    }

    public async Task SetAsync(string key, string value)
    {
        await _db.StringSetAsync(key, value, TimeSpan.FromHours(24));
    }
}