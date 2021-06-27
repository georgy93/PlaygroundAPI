namespace Playground.Infrastructure.Identity.Services
{
    using Domain.Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    internal class IdentityService : IIdentityService
    {
        private readonly ILogger<IdentityService> _logger;
        // private readonly IAuthenticationGateway _authenticationGateWay;
        private readonly IOptionsMonitor<JwtSettings> _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public IdentityService(ILogger<IdentityService> logger,
                               // IAuthenticationGateway authenticationGateWay,
                               IOptionsMonitor<JwtSettings> jwtSettings,
                               UserManager<ApplicationUser> userManager,
                               TokenValidationParameters tokenValidationParameters)
        {
            _logger = logger;
            //_authenticationGateWay = authenticationGateWay;
            _jwtSettings = jwtSettings;
            _userManager = userManager;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<AuthenticationResult> RegisterAsync(UserRegistrationRequest request)
        {
            var email = request.Email;

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser is not null)
                return AuthenticationResult.Fail(IdentityErrors.EmailIsAlreadyTakenByAnotherUser);

            var newUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                UserName = email
            };

            // await userManager.AddClaimAsync(newUser, new Claim("tags.view", "true"));

            var createdUser = await _userManager.CreateAsync(newUser, request.Password);

            return createdUser.Succeeded
                ? await GenerateAuthenticationResultForUserAsync(newUser)
                : AuthenticationResult.Fail(createdUser.Errors.Select(x => x.Description).ToArray());
        }

        public async Task<AuthenticationResult> LoginAsync(UserLoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return AuthenticationResult.Fail(IdentityErrors.UserDoesNotExist);

            return await _userManager.CheckPasswordAsync(user, request.Password)
                ? await GenerateAuthenticationResultForUserAsync(user)
                : AuthenticationResult.Fail(IdentityErrors.InvalidUserPasswordCombination);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var validatedToken = GetPrincipalFromToken(request.Token);
            if (validatedToken is null)
                return AuthenticationResult.Fail(IdentityErrors.InvalidToken);

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
                return AuthenticationResult.Fail(IdentityErrors.TokenHasntExpiredYet);

            // TODO Uncomment when ready
            //var storedRefreshToken = await _authenticationGateWay.GetRefreshToken(request.RefreshToken);
            //if (storedRefreshToken is null)
            //    return AuthenticationResult.Fail(IdentityErrors.TokenDoesNotExist);

            //if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            //    return AuthenticationResult.Fail(IdentityErrors.TokenHasExpired);

            //if (storedRefreshToken.Invalidated)
            //    return AuthenticationResult.Fail(IdentityErrors.RefreshTokenIsInvalidated);

            //if (storedRefreshToken.Used)
            //    return AuthenticationResult.Fail(IdentityErrors.RefreshedTokenIsAlreadyUsed);

            //var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            //if (storedRefreshToken.JwtId is not jti)
            //    return AuthenticationResult.Fail(IdentityErrors.RefreshTokenDoesNotMatchJWT);

            //storedRefreshToken.Used = true;

            //await _authenticationGateWay.UpdateRefreshToken(storedRefreshToken);

            var userId = validatedToken.Claims.Single(x => x.Type == "id").Value;
            var user = await _userManager.FindByIdAsync(userId);

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);

                if (IsJwtWithValidSecurityAlgorithm(validatedToken))
                    return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during Token Validation");
            }

            return null;
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, user.Email),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (JwtRegisteredClaimNames.Email, user.Email),
                new ("id", user.Id),
            };

            var userClaims = await _userManager.GetClaimsAsync(user);

            claims.AddRange(userClaims);

            var key = Encoding.ASCII.GetBytes(_jwtSettings.CurrentValue.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.CurrentValue.TokenLifetime.Minutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddHours(2)
            };

            // await _authenticationGateWay.AddRefreshToken(refreshToken);

            var tokenString = tokenHandler.WriteToken(token);

            return AuthenticationResult.Success(tokenString, refreshToken.Token);
        }

        private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken) => validatedToken
            is JwtSecurityToken jwtSecurityToken
            && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
    }
}