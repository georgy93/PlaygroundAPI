﻿namespace Playground.API.Behavior.Filters;

using DTOs;

public class ModelValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ModelState.IsValid)
        {
            await next();
            return;
        }

        var validationError = context.ModelState.First();

        context.Result = new BadRequestObjectResult(new ErrorResponse
        {
            ErrorCode = validationError.Key,
            Description = validationError.Value.Errors[0].ErrorMessage
        });
    }
}