using CareSync.ApplicationLayer.IServices.ICacheService;
using Microsoft.Extensions.Caching.Memory;

namespace CareSync.InfrastructureLayer.Services.CacheService;

public class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    public Task<T?> GetAsync<T>(string key)
    {
        memoryCache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan duration)
    {
        memoryCache.Set(key, value, duration);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        memoryCache.Remove(key);
        return Task.CompletedTask;
    }
}
