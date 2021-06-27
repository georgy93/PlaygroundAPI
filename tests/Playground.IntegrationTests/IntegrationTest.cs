namespace Playground.IntegrationTests
{
    using API;
    using Infrastructure.Identity.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Mvc.Testing;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Xunit;

    public abstract class IntegrationTest : IDisposable
    {
        protected IntegrationTest()
        {
            WebApplicationFactory = new CustomWebApplicationFactory();
            TestClient = WebApplicationFactory.CreateClient();
        }

        protected HttpClient TestClient { get; }

        protected WebApplicationFactory<Startup> WebApplicationFactory { get; }

        public virtual void Dispose()
        {
            WebApplicationFactory.Dispose();
            GC.SuppressFinalize(this);
        }

        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, await GetJwtAsync());
        }

        private async Task<string> GetJwtAsync()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new UserRegistrationRequest
            {
                Email = "test@integration.com",
                Password = "somePassword1234!"
            });

            var authenticationResult = await response.Content.ReadAsAsync<AuthSuccessResponse>();

            return authenticationResult.Token;
        }
    }
}