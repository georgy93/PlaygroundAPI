namespace Playground.API.Behavior.Middlewares
{
    using Application.Common;
    using Domain.Exceptions;
    using Domain.Exceptions.Serialization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Settings;
    using System;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;
    using Utils.Extensions;

    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IOptionsMonitor<ErrorHandlingSettings> _errorHandlingSettings;
        private readonly IActionResultExecutor<ObjectResult> _executor;
        private readonly BusinessExceptionContractResolver _businessExceptionContractResolver;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next,
                                                 IOptionsMonitor<ErrorHandlingSettings> errorHandlingSettings,
                                                 IActionResultExecutor<ObjectResult> executor,
                                                 ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _errorHandlingSettings = errorHandlingSettings;
            _executor = executor;
            _logger = logger;
            _businessExceptionContractResolver = new BusinessExceptionContractResolver();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException buisnessEx)
            {
                var logData = new { context.Request.Path, Error = buisnessEx };
                _logger.LogError(buisnessEx, logData.Stringify(_businessExceptionContractResolver));

                await TrySetResponseAsync(context, buisnessEx, buisnessEx.HttpStatusCode);
            }
            catch (Exception ex)
            {
                var logData = new { context.Request.Path, Error = ex };
                _logger.LogError(ex, logData.Stringify());

                await TrySetResponseAsync(context, ex, StatusCodes.Status500InternalServerError);
            }
        }

        private async Task TrySetResponseAsync(HttpContext context, Exception exception, int statusCode)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogError($"Cannot handle error. The response has already started.");

                ExceptionDispatchInfo.Throw(exception);
            }

            var errorResponse = CreateErrorResponse(exception);

            await context.WriteResultAsync(_executor, errorResponse, statusCode);
        }

        private ErrorResponse CreateErrorResponse(Exception exception)
        {
            var errorCode = "InternalServerError";
            var description = "InternalServerError";

            if (exception is BusinessException)
            {
                //  errorCode = businessException.ErrorCode;
                //   description = businessException.Description;
            }

            return new(errorCode, description, _errorHandlingSettings.CurrentValue.ShowDetails ? exception : null);
        }
    }
}