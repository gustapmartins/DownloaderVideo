using DownloaderVideo.Domain.Exceptions;
using DownloaderVideo.Domain.Interface.Services;
using Microsoft.Extensions.Caching.Memory;

namespace VarzeaLeague.Domain.Service;

public class MemoryCacheService : IMemoryCacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void AddToCache<Output>(string keyMemoryCache, Output objectCache, int timeCache)
    {
        MemoryCacheEntryOptions options = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(timeCache)
        };

        if (objectCache != null)
        {
            _memoryCache.Set(keyMemoryCache, objectCache, options);
        }
    }

    public Output GetCache<Output>(string keyMemoryCache)
    {
        try
        {
            _memoryCache.TryGetValue(keyMemoryCache, out Output returnCache);

            return returnCache!;

        }catch (ExceptionFilter ex) 
        {
            throw new ExceptionFilter(ex.Message, ex);
        }
    }

    public void RemoveFromCache<Output>(string key)
    {
        _memoryCache.Remove(key);
    }

    public Output GetOrCreate<Output>(string key, Func<Output> function, int expirationTime)
    {
        try
        {
            if (_memoryCache.TryGetValue(key, out Output cachedItem))
               return cachedItem!;

            MemoryCacheEntryOptions options = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationTime)
            };

            Output registerCache = function.Invoke();

            if (registerCache != null)
               _memoryCache.Set(key, registerCache, options);

            return registerCache;

        }catch(ExceptionFilter ex) 
        {
            throw new ExceptionFilter(ex.Message, ex);
        }
    }
}
