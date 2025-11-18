namespace CareSync.ApplicationLayer.IServices.ICacheService;

internal interface ICacheService
{
    T? GetFromCacheAsync<T>(string key);
    void SetCacheAsync<T>(string key, T value, TimeSpan duration);
    void RemoveFromCacheAsync(string key);
}
