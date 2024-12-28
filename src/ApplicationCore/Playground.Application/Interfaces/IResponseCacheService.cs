namespace Playground.Application.Interfaces;

public interface IResponseCacheService
{
    string GetCachedResponse(string key);

    void CacheResponse(string key, object response, TimeSpan timeToLive);

    void RemoveCacheResponse(string key);

    public static bool IsValidResponse(string response) => !string.IsNullOrEmpty(response);
}