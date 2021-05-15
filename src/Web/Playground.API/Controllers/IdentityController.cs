﻿namespace Playground.API.Controllers
{
    using Infrastructure.Identity.Models;
    using Infrastructure.Identity.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [Route("identity")]
    [ProducesResponseType(typeof(AuthSuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthFailedResponse), StatusCodes.Status400BadRequest)]
    public class IdentityController : BaseController
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService/*, IMapper mapper*/)
        // : base(mapper)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// Registers a user in the system
        /// </summary>
        /// <response code="200">Registers a user in the system</response>
        /// <response code="400">Unable to register the user due to a validation error</response>
        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest registrationRequest)
        {
            var authResponse = await _identityService.RegisterAsync(registrationRequest);

            return authResponse
                ? Ok(new AuthSuccessResponse(authResponse.Token, authResponse.RefreshToken))
                : BadRequest(new AuthFailedResponse(authResponse.Errors));
        }

        /// <summary>
        /// Logins a user in the system
        /// </summary>
        /// <response code="200">Logins a user in the system</response>
        /// <response code="400">Unable to login the user due to a validation error</response>
        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            var authResponse = await _identityService.LoginAsync(loginRequest);

            return authResponse
                ? Ok(new AuthSuccessResponse(authResponse.Token, authResponse.RefreshToken))
                : BadRequest(new AuthFailedResponse(authResponse.Errors));
        }

        /// <summary>
        /// Refreshes a user token
        /// </summary>
        /// <response code="200">Refreshes a user token</response>
        /// <response code="400">Unable to refresh a user token due to a validation error</response>
        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var authResponse = await _identityService.RefreshTokenAsync(refreshTokenRequest);

            return authResponse
                ? Ok(new AuthSuccessResponse(authResponse.Token, authResponse.RefreshToken))
                : BadRequest(new AuthFailedResponse(authResponse.Errors));
        }
    }
}