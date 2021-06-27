namespace Playground.IntegrationTests
{
    using API;
    using API.Behavior.Settings;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public sealed class SecretControllerTests : IntegrationTest
    {
        private static readonly string ApiKeyHeaderName = "ApiKey";

        [Fact]
        public async Task Caling_SecretController_Get_WithCorrectApiKey_Should_SucceedAsync()
        {
            // Arrange
            var apiKeySettings = WebApplicationFactory.Services.GetRequiredService<IOptions<ApiKeySettings>>();
            var apiKey = apiKeySettings.Value.Key;

            // Act
            var response = await SendRequestToSecretGetWithApiKeyAsync(apiKey);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Caling_SecretController_Get_WithIncorrectApiKey_Should_FailAsync()
        {
            // Arrange
            var apiKeySettings = WebApplicationFactory.Services.GetRequiredService<IOptions<ApiKeySettings>>();
            var wrongApiKey = apiKeySettings.Value.Key + "!@#";

            // Act
            var response = await SendRequestToSecretGetWithApiKeyAsync(wrongApiKey);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Caling_SecretController_Get_WithMissingApiKey_Should_FailAsync()
        {
            // Arrange
            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Secret.Get);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private Task<HttpResponseMessage> SendRequestToSecretGetWithApiKeyAsync(string apiKey)
        {
            TestClient.DefaultRequestHeaders.Add(ApiKeyHeaderName, apiKey);

            return TestClient.GetAsync(ApiRoutes.Secret.Get);
        }
    }
}