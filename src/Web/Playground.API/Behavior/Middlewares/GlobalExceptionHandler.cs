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
    private readonly BusinessExceptionContractResolver _businessExceptionContractResolver;

    public GlobalExceptionHandler(IOptionsMonitor<ErrorHandlingSettings> errorHandlingSettings, ILogger<GlobalExceptionHandler> logger)
    {
        _errorHandlingSettings = errorHandlingSettings;
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

        _logger.LogError(businessException, "{Exception}", logData.Beautify(_businessExceptionContractResolver));

        var errorResponse = CreateErrorResponse(businessException);

        context.Response.StatusCode = businessException.HttpStatusCode;

        await context.Response.WriteAsJsonAsync(errorResponse);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "{Path}", context.Request.Path);

        var errorResponse = CreateDefaultErrorResponse(exception);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await context.Response.WriteAsJsonAsync(errorResponse);
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