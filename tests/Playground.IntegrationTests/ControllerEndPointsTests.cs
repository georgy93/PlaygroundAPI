namespace Playground.IntegrationTests
{
    using API;
    using API.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;
    using System.Reflection;
    using Xunit;

    public class ControllerEndPointsTests
    {
        [Fact]
        public void SecretController_Should_Contain_Correct_Endpoints()
        {
            // Arrange
            // Act
            var controller = new SecretController();

            // Assert
            ValidateEndpoint(ApiRoutes.Secret.Get, controller.GetEndpointMetaData<HttpGetAttribute>(nameof(SecretController.GetSecret)));
            ValidateEndpoint(ApiRoutes.Secret.CancellationTokenMap, controller.GetEndpointMetaData<HttpGetAttribute>(nameof(SecretController.MapRequestAbortedToCancellationTokenParameterAsync)));
        }

        [Fact]
        public void IdentityController_Should_Contain_Correct_Endpoints()
        {
            // Arrange
            // Act
            var controller = new IdentityController(null);

            // Assert
            ValidateEndpoint(ApiRoutes.Identity.Register, controller.GetEndpointMetaData<HttpPostAttribute>(nameof(IdentityController.RegisterAsync)));
            ValidateEndpoint(ApiRoutes.Identity.Login, controller.GetEndpointMetaData<HttpPostAttribute>(nameof(IdentityController.LoginAsync)));
            ValidateEndpoint(ApiRoutes.Identity.Refresh, controller.GetEndpointMetaData<HttpPostAttribute>(nameof(IdentityController.RefreshTokenAsync)));
        }

        private static void ValidateEndpoint(string url, HttpMethodAttribute endpointHttpAtribute)
        {
            Assert.NotNull(endpointHttpAtribute);
            Assert.Equal(url, endpointHttpAtribute.Template);
        }
    }

    internal static class Extensions
    {
        internal static T GetEndpointMetaData<T>(this ControllerBase controller, string methodName) where T : HttpMethodAttribute
            => controller
            .GetType()
            .GetMethod(methodName)
            .GetCustomAttribute<T>();
    }
}