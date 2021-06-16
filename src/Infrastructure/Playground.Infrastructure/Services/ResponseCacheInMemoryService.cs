namespace Playground.Infrastructure.Services
{
    using Application.Interfaces;
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Text.Json;

    internal class ResponseCacheInMemoryService : IResponseCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public ResponseCacheInMemoryService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public string GetCachedResponse(string Key) => _memoryCache.Get<string>(Key);

        public void RemoveCacheResponse(string key) => _memoryCache.Remove(key);

        public void CacheResponse(string key, object response, TimeSpan timeToLive)
        {
            if (response is null)
                return;

            var serialzedResponse = JsonSerializer.Serialize(response);

            _memoryCache.Set(key, serialzedResponse, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeToLive
            });
        }
    }
}