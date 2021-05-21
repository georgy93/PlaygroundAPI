﻿namespace Playground.Application.Interfaces
{
    using System;

    public interface IResponseCacheService
    {
        string GetCachedResponse(string cacheKey);

        void CacheResponse(string cacheKey, object response, TimeSpan timeToLive);

        void RemoveCacheResponse(string key);

        public bool IsValidResponse(string response) => !string.IsNullOrEmpty(response);
    }
}