namespace Playground.API.Behavior.Middlewares;

using Application.Common.Serialization;
using Domain.Exceptions;
using DTOs;
using Microsoft.AspNetCore.Diagnostics;
using Settings;
using System.Threading;
using Utils.Extensions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IOptionsMonitor<ErrorHandlingSettings> _errorHandlingSettings;
    private readonly IActionResultExecutor<ObjectResult> _executor;
    private readonly BusinessExceptionContractResolver _businessExceptionContractResolver;

    public GlobalExceptionHandler(IOptionsMonitor<ErrorHandlingSettings> errorHandlingSettings,
                                  IActionResultExecutor<ObjectResult> executor,
                                  ILogger<GlobalExceptionHandler> logger)
    {
        _errorHandlingSettings = errorHandlingSettings;
        _executor = executor;
        _logger = logger;
        _businessExceptionContractResolver = new BusinessExceptionContractResolver();
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (!CanHandleException(httpContext))
            return false;

        if (exception is BusinessException businessEx)
            await HandleBusinessExceptionAsync(httpContext, businessEx);
        else
            await HandleExceptionAsync(httpContext, exception);

        return true;
    }

    private bool CanHandleException(HttpContext context)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Cannot handle error. The response has already started.");

            return false;
        }

        return true;
    }

    private async Task HandleBusinessExceptionAsync(HttpContext context, BusinessException businessException)
    {
        var logData = new { context.Request.Path, Error = businessException };

        _logger.LogError(businessException, "{exception}", logData.Beautify(_businessExceptionContractResolver));

        var errorResponse = CreateErrorResponse(businessException);

        await context.WriteResultAsync(_executor, errorResponse, businessException.HttpStatusCode);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "{path}", context.Request.Path);

        var errorResponse = CreateDefaultErrorResponse(exception);

        await context.WriteResultAsync(_executor, errorResponse, StatusCodes.Status500InternalServerError);
    }

    private ErrorResponse CreateErrorResponse(BusinessException businessException) => new()
    {
        //ErrorCode = businessException.ErrorCode,
        //Description = businessException.Message,
        Exception = _errorHandlingSettings.CurrentValue.ShowDetails ? businessException : null
    };

    private ErrorResponse CreateDefaultErrorResponse(Exception exception) => new()
    {
        ErrorCode = "InternalServerError",
        Description = "InternalServerError",
        Exception = _errorHandlingSettings.CurrentValue.ShowDetails ? exception : null
    };
}