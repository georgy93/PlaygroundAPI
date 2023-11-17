namespace Playground.API.Behavior.Filters;

using Application.Interfaces;
using Utils.Extensions;

public sealed class RemoveCachedResponseAttribute : TypeFilterAttribute
{
    public RemoveCachedResponseAttribute() : base(typeof(RemoveCachedResponseImplementationAttribute)) { }

    public sealed class RemoveCachedResponseImplementationAttribute : ActionFilterAttribute
    {
        private readonly IResponseCacheService _cacheService;

        public RemoveCachedResponseImplementationAttribute(IResponseCacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var key = context.HttpContext.GenerateCacheKeyFromRequest();

            _cacheService.RemoveCacheResponse(key);
        }
    }
}