namespace Playground.IntegrationTests.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;
using Xunit;

internal static class EndpointExtensions
{
    internal static void ValidateEndpoint<THttpMethodAttribute>(this ControllerBase controller, string endpointName, string url)
        where THttpMethodAttribute : HttpMethodAttribute
    {
        var endpointHttpAtribute = controller.GetEndpointMetaData<THttpMethodAttribute>(endpointName);

        Assert.NotNull(endpointHttpAtribute);
        Assert.Equal(endpointHttpAtribute.Template, url);
    }

    private static THttpMethodAttribute GetEndpointMetaData<THttpMethodAttribute>(this ControllerBase controller, string methodName)
        where THttpMethodAttribute : HttpMethodAttribute
        => controller
        .GetType()
        .GetMethod(methodName)
        .GetCustomAttribute<THttpMethodAttribute>();
}