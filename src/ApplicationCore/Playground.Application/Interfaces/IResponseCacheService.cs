namespace Playground.Application.Interfaces;

public interface IResponseCacheService
{
    string GetCachedResponse(string cacheKey);

    void CacheResponse(string cacheKey, object response, TimeSpan timeToLive);

    void RemoveCacheResponse(string key);

    public static bool IsValidResponse(string response) => !string.IsNullOrEmpty(response);
}