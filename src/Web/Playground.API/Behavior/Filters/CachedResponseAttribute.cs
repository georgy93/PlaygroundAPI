namespace Playground.API.Behavior.Filters
{
    using Application.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Utils.Extensions;

    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class CachedResponseAttribute : TypeFilterAttribute
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

                var cacheKey = context.HttpContext.GenerateCacheKeyFromRequest();
                var cachedResponse = _cacheService.GetCachedResponse(cacheKey);

                if (IResponseCacheService.IsValidResponse(cachedResponse))
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

        // TODO: Use other return type
        private static ContentResult CreateOkContentResult(string data) => new()
        {
            Content = data,
            ContentType = MediaTypeNames.Application.Json,
            StatusCode = StatusCodes.Status200OK
        };
    }
}