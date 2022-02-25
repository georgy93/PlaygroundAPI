namespace Playground.IntegrationTests
{
    using API;
    using Configuration;
    using Infrastructure.Identity.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Mvc.Testing;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                WebApplicationFactory.Dispose();
                TestClient.Dispose();
            }
        }

        protected async Task AuthenticateAsync()
        {
            var jwt = await GetJwtAsync();

            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwt);
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

        ~IntegrationTest()
        {
            Dispose(false);
        }
    }
}