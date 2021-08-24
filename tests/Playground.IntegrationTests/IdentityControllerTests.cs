namespace Playground.IntegrationTests
{
    using API;
    using API.Controllers;
    using Extensions;
    using Infrastructure.Identity;
    using Infrastructure.Identity.Models;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Xunit;

    public class IdentityControllerTests : IntegrationTest
    {
        private readonly string CorrectPasswrod = "somePassword1234!";

        [Fact]
        public void IdentityController_Should_Contain_Correct_Endpoints()
        {
            // Arrange
            // Act
            var controller = new IdentityController(null);

            // Assert
            controller.ValidateEndpoint<HttpPostAttribute>(nameof(IdentityController.RegisterAsync), ApiRoutes.Identity.Register);
            controller.ValidateEndpoint<HttpPostAttribute>(nameof(IdentityController.LoginAsync), ApiRoutes.Identity.Login);
            controller.ValidateEndpoint<HttpPostAttribute>(nameof(IdentityController.RefreshTokenAsync), ApiRoutes.Identity.Refresh);
        }

        [Fact]
        public async Task Caling_IdentityController_Register_With_CorrectDataForNewUser_Should_RegisterTheNewUserAsync()
        {
            // Arrange
            var userRegistrationRequest = new UserRegistrationRequest
            {
                Email = GenerateEmail(),
                Password = CorrectPasswrod
            };

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, userRegistrationRequest);
            var authenticationResult = await response.Content.ReadAsAsync<AuthSuccessResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(authenticationResult.Token);
        }

        [Fact]
        public async Task Caling_IdentityController_Register_With_AlreadyRegisteredEmail_Should_FailTheRegistrationAsync()
        {
            // Arrange
            var userRegistrationRequest = new UserRegistrationRequest
            {
                Email = GenerateEmail(),
                Password = CorrectPasswrod
            };

            // Act
            await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, userRegistrationRequest);

            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, userRegistrationRequest);
            var authenticationResult = await response.Content.ReadAsAsync<AuthFailedResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(IdentityErrors.EmailIsAlreadyTakenByAnotherUser, authenticationResult.Errors);
        }

        [Fact]
        public async Task Caling_IdentityController_Login__With_RegisteredUser_Should_LoginTheUserAsync()
        {
            // Arrange
            var email = GenerateEmail();

            var userRegistrationRequest = new UserRegistrationRequest
            {
                Email = email,
                Password = CorrectPasswrod
            };

            var userLoginRequest = new UserLoginRequest
            {
                Email = email,
                Password = CorrectPasswrod
            };

            // Act
            await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, userRegistrationRequest);

            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, userLoginRequest);
            var authenticationResult = await response.Content.ReadAsAsync<AuthSuccessResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(authenticationResult.Token);
        }

        [Fact]
        public async Task Caling_IdentityController_Login__With_NonRegisteredUser_Should_FailTheLoginAsync()
        {
            // Arrange
            var userLoginRequest = new UserLoginRequest
            {
                Email = GenerateEmail(),
                Password = CorrectPasswrod
            };

            // Act
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, userLoginRequest);
            var authenticationResult = await response.Content.ReadAsAsync<AuthFailedResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(IdentityErrors.UserDoesNotExist, authenticationResult.Errors);
        }

        [Fact]
        public async Task Caling_IdentityController_Login__With_InvalidPassword_Should_FailTheLoginAsync()
        {
            // Arrange
            var email = GenerateEmail();

            var userRegistrationRequest = new UserRegistrationRequest
            {
                Email = email,
                Password = CorrectPasswrod
            };

            var userLoginRequest = new UserLoginRequest
            {
                Email = email,
                Password = CorrectPasswrod + "!@#$"
            };

            // Act
            await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, userRegistrationRequest);

            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, userLoginRequest);
            var authenticationResult = await response.Content.ReadAsAsync<AuthFailedResponse>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(IdentityErrors.InvalidUserPasswordCombination, authenticationResult.Errors);
        }

        private static string GenerateEmail()
        {
            var rnd = new Random();
            var number = rnd.Next(1, 100000);

            return $"someEmail{number}@yahoo.com";
        }
    }
}