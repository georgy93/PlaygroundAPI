namespace Playground.API.Behavior.Extensions
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Routing;
    using System.Threading.Tasks;

    // TODO use the better method
    public static class HttpContextExtensions
    {
        private static readonly RouteData EmptyRouteData = new();
        private static readonly ActionDescriptor EmptyActionDescriptor = new();

        public static Task WriteResultAsync<TResult>(this HttpContext context, IActionResultExecutor<TResult> resultExecutor, TResult result)
            where TResult : IActionResult
        {
            var routeData = context.GetRouteData() ?? EmptyRouteData;
            var actionContext = new ActionContext(context, routeData, EmptyActionDescriptor);

            return resultExecutor.ExecuteAsync(actionContext, result);
        }
    }
}