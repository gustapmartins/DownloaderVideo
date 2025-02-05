namespace DownloaderVideo.Domain.Interface.Services;

public interface IMemoryCacheService
{
    Output GetCache<Output>(string keyMemoryCache);
    void AddToCache<Output>(string keyMemoryCache, Output objectCache, int timeCache);
    void RemoveFromCache<Output>(string key);
    Output GetOrCreate<Output>(string key, Func<Output> function, int expirationTime);
}
