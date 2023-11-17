namespace Playground.API.Behavior.Middlewares;

using Application.Common.Serialization;
using Domain.Exceptions;
using DTOs;
using Settings;
using System.Runtime.ExceptionServices;
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
        catch (BusinessException businessEx)
        {
            await HandleBusinessExceptionAsync(context, businessEx);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleBusinessExceptionAsync(HttpContext context, BusinessException businessException)
    {
        var logData = new { context.Request.Path, Error = businessException };

        _logger.LogError(businessException, "{exception}", logData.Beautify(_businessExceptionContractResolver));

        ReThrowIfResponseHasStarted(context, businessException);

        var errorResponse = CreateErrorResponse(businessException);

        await context.WriteResultAsync(_executor, errorResponse, businessException.HttpStatusCode);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "{path}", context.Request.Path);

        ReThrowIfResponseHasStarted(context, exception);

        var errorResponse = CreateDefaultErrorResponse(exception);

        await context.WriteResultAsync(_executor, errorResponse, StatusCodes.Status500InternalServerError);
    }

    private void ReThrowIfResponseHasStarted(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning($"Cannot handle error. The response has already started.");
            ExceptionDispatchInfo.Throw(exception);
        }
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