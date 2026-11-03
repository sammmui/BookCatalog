using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace BookCatalog.Services;

public class ApiCache
{
    private readonly IMemoryCache _cache;
    private CancellationTokenSource _reset = new();

    public ApiCache(IMemoryCache cache) => _cache = cache;

    public bool TryGet<T>(string key, out T? value) => _cache.TryGetValue(key, out value);

    public void Set<T>(string key, T value)
    {
        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
            .AddExpirationToken(new CancellationChangeToken(_reset.Token));
        _cache.Set(key, value, options);
    }

    public void Clear()
    {
        var old = Interlocked.Exchange(ref _reset, new CancellationTokenSource());
        old.Cancel();
        old.Dispose();
    }
}