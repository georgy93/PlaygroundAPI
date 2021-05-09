namespace Playground.API.Behavior.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Options;
    using Settings;
    using System;
    using System.Threading.Tasks;

    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : TypeFilterAttribute
    {
        public ApiKeyAuthAttribute() : base(typeof(ApiKeyAuthAttributeImplementation)) { }

        public class ApiKeyAuthAttributeImplementation : IAsyncActionFilter
        {
            private const string ApiKeyHeaderName = "ApiKey";

            private readonly IOptionsSnapshot<ApiKeySettings> _apiKeySettings;

            public ApiKeyAuthAttributeImplementation(IOptionsSnapshot<ApiKeySettings> apiKeySettings)
            {
                _apiKeySettings = apiKeySettings;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (AuthenticationKeyIsValid(context))
                    await next();
                else
                    context.Result = new UnauthorizedResult();
            }

            private bool AuthenticationKeyIsValid(ActionExecutingContext context) =>
                context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey)
                && _apiKeySettings.Value.Equals(potentialApiKey);
        }
    }
}