namespace Playground.API.Behavior.Middlewares
{
    using Domain.Exceptions;
    using Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Settings;
    using System;
    using System.Threading.Tasks;

    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<ErrorHandlingConfiguration> _config;
        private readonly IActionResultExecutor<ObjectResult> _executor;
        // private readonly BusinessExceptionContractResolver _contractResolver;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next,
                                                 IOptionsMonitor<ErrorHandlingConfiguration> config,
                                                 IActionResultExecutor<ObjectResult> executor,
                                                 ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _config = config;
            _executor = executor;
            _logger = logger;
            // _contractResolver = new BusinessExceptionContractResolver();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException buisnessEx)
            {

            }
            catch (Exception ex)
            {
                // LogRequestError(ex, context);

                if (context.Response.HasStarted)
                {
                    _logger.LogError($"Cannot handle error. The response has already started.");
                    throw;
                }

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            await context.WriteResultAsync(_executor, new OkObjectResult(""));
        }
    }
}