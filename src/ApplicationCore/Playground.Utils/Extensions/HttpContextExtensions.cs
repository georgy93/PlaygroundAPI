namespace Playground.Utils.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using System.Text;

// TODO use the better method
public static class HttpContextExtensions
{
    private static readonly RouteData EmptyRouteData = new();
    private static readonly ActionDescriptor EmptyActionDescriptor = new();

    extension(HttpContext context)
    {
        public async Task WriteResultAsync<TResult>(IActionResultExecutor<ObjectResult> resultExecutor,
                                                           TResult result,
                                                           int statusCode = StatusCodes.Status500InternalServerError)
        {
            var routeData = context.GetRouteData() ?? EmptyRouteData;
            var actionContext = new ActionContext(context, routeData, EmptyActionDescriptor);
            var objectResult = new ObjectResult(result)
            {
                StatusCode = statusCode,
                DeclaredType = typeof(TResult)
            };

            await resultExecutor.ExecuteAsync(actionContext, objectResult);
        }

        public string GenerateCacheKeyFromRequest()
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append($"{context.Request.Path}");

            foreach (var (key, value) in context.Request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}