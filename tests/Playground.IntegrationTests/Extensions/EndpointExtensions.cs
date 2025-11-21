namespace Playground.IntegrationTests.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;
using Xunit;

internal static class EndpointExtensions
{
    extension(ControllerBase controller)
    {
        internal void ValidateEndpoint<THttpMethodAttribute>(string endpointName, string url)
            where THttpMethodAttribute : HttpMethodAttribute
        {
            var endpointHttpAtribute = controller.GetEndpointMetaData<THttpMethodAttribute>(endpointName);

            Assert.NotNull(endpointHttpAtribute);
            Assert.Equal(endpointHttpAtribute.Template, url);
        }

        private THttpMethodAttribute GetEndpointMetaData<THttpMethodAttribute>(string methodName)
            where THttpMethodAttribute : HttpMethodAttribute
            => controller
            .GetType()
            .GetMethod(methodName)
            .GetCustomAttribute<THttpMethodAttribute>();
    }
}