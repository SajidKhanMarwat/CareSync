using CareSync.ApplicationLayer.IServices.ICacheService;
using Microsoft.Extensions.Caching.Memory;

namespace CareSync.InfrastructureLayer.Services.CacheService;

public sealed class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    public T? GetFromCacheAsync<T>(string key)
    {
        memoryCache.TryGetValue(key, out T? value);
        return value;
    }

    public void SetCacheAsync<T>(string key, T value, TimeSpan duration)
    {
        memoryCache.Set(key, value, duration);
    }

    public void RemoveFromCacheAsync(string key)
    {
        memoryCache.Remove(key);
    }
}
