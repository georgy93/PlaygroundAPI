namespace Playground.API.Behavior.Filters
{
    using Application.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class RemoveCachedResponseAttribute : TypeFilterAttribute
    {
        public RemoveCachedResponseAttribute() : base(typeof(RemoveCachedResponseImplementationAttribute)) { }

        public class RemoveCachedResponseImplementationAttribute : ActionFilterAttribute
        {
            private readonly IResponseCacheService _cacheService;

            public RemoveCachedResponseImplementationAttribute(IResponseCacheService cacheService)
            {
                _cacheService = cacheService;
            }

            public override void OnActionExecuting(ActionExecutingContext context)
            {
                var key = context.HttpContext.Request.GenerateCacheKeyFromRequest();

                _cacheService.RemoveCacheResponse(key);
            }
        }
    }
}