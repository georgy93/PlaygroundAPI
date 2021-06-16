namespace Playground.IntegrationTests
{
    using API;
    using API.Behavior.Settings;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Xunit;

    public sealed class SecretControllerTests : IClassFixture<AppTestFixture>, IDisposable
    {
        private static readonly string ApiKeyHeaderName = "ApiKey";

        private readonly AppTestFixture _fixture;
        private readonly HttpClient _client;

        public SecretControllerTests(AppTestFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        [Fact]
        public async Task Caling_SecretController_Get_WithCorrectApiKey_ShouldSucceedAsync()
        {
            // Arrange
            var apiKeySettings = _fixture.Services.GetRequiredService<IOptions<ApiKeySettings>>();
            var apiKey = apiKeySettings.Value.Key;

            // Act
            var response = await SendRequestToSecretGetWithApiKeyAsync(apiKey);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task Caling_SecretController_Get_WithIncorrectApiKey_ShouldFailAsync()
        {
            // Arrange
            var apiKeySettings = _fixture.Services.GetRequiredService<IOptions<ApiKeySettings>>();
            var wrongApiKey = apiKeySettings.Value.Key + "!@#";

            // Act
            var response = await SendRequestToSecretGetWithApiKeyAsync(wrongApiKey);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task Caling_SecretController_Get_WithMissingApiKey_ShouldFailAsync()
        {
            // Arrange
            // Act
            var response = await _client.GetAsync(ApiRoutes.Secret.Get);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType.MediaType);
        }

        private Task<HttpResponseMessage> SendRequestToSecretGetWithApiKeyAsync(string apiKey)
        {
            _client.DefaultRequestHeaders.Add(ApiKeyHeaderName, apiKey);

            return _client.GetAsync(ApiRoutes.Secret.Get);
        }
    }
}