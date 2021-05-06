namespace Playground.API.Behavior.Middlewares
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using System;
    using System.Threading.Tasks;

    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        //  private readonly ILogger _logger = Log.Logger.ForContext<GlobalExceptionHandlingMiddleware>();
        //  private readonly IOptionsMonitor<ErrorHandlingConfiguration> _config;
        private readonly IActionResultExecutor<ObjectResult> _executor;
        // private readonly BusinessExceptionContractResolver _contractResolver;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, /*IOptionsMonitor<ErrorHandlingConfiguration> config,*/ IActionResultExecutor<ObjectResult> executor)
        {
            _next = next;
            //_config = config;
            _executor = executor;
            // _contractResolver = new BusinessExceptionContractResolver();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // LogRequestError(ex, context);

                if (context.Response.HasStarted)
                {
                    // _logger.Warning($"Cannot handle error. The response has already started.");
                    throw;
                }

                //  await HandleExceptionAsync(context, ex);
            }
        }
    }
}