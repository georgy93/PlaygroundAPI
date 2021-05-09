﻿namespace Playground.API.Behavior.Filters
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System;
    using System.Threading.Tasks;

    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class CachedResponseAttribute : TypeFilterAttribute
    {
        public CachedResponseAttribute(int timeToLiveSeconds) : base(typeof(CachedResponseAttributeImplementation))
        {
            Arguments = new object[] { timeToLiveSeconds };
        }

        public class CachedResponseAttributeImplementation : IAsyncActionFilter
        {
            private readonly int _timeToLiveSeconds;
            private readonly IResponseCacheService _cacheService;

            public CachedResponseAttributeImplementation(int timeToLiveSeconds, IResponseCacheService cacheService)
            {
                _cacheService = cacheService;
                _timeToLiveSeconds = timeToLiveSeconds;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                // Before action

                string cacheKey = context.HttpContext.Request.GenerateCacheKeyFromRequest();
                string cachedResponse = _cacheService.GetCacheResponse(cacheKey);

                if (_cacheService.IsValidResponse(cachedResponse))
                {
                    context.Result = CreateOkContentResult(cachedResponse);
                    return;
                }

                var executedContext = await next();

                // After action

                if (executedContext.Result is OkObjectResult okObjectResult)
                {
                    _cacheService.CacheResponse(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
                }
            }
        }

        private static ContentResult CreateOkContentResult(string data)
        {
            return new ContentResult()
            {
                Content = data,
                ContentType = "application/json",
                StatusCode = StatusCodes.Status200OK
            };
        }
    }
}