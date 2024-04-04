﻿namespace Playground.Infrastructure.Identity.Services;

using Application.Common;
using Domain.Entities;
using Domain.Entities.Aggregates.BuyerAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

internal class IdentityService : IIdentityService
{
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<IdentityService> _logger;
    // private readonly IAuthenticationGateway _authenticationGateWay;
    private readonly IOptionsMonitor<JwtSettings> _jwtSettings;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public IdentityService(ILogger<IdentityService> logger,
                           // IAuthenticationGateway authenticationGateWay,
                           IOptionsMonitor<JwtSettings> jwtSettings,
                           UserManager<ApplicationUser> userManager,
                           TokenValidationParameters tokenValidationParameters,
                           TimeProvider timeProvider)
    {
        _logger = logger;
        //_authenticationGateWay = authenticationGateWay;
        _jwtSettings = jwtSettings;
        _userManager = userManager;
        _tokenValidationParameters = tokenValidationParameters;
        _timeProvider = timeProvider;
    }

    public async Task<AuthenticationResult> RegisterAsync(UserRegistrationRequest request)
    {
        var email = Email.FromString(request.Email);

        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser is not null)
            return AuthenticationResult.Fail(IdentityErrors.EmailIsAlreadyTakenByAnotherUser);

        var userId = new UserId(Guid.NewGuid());
        var newUser = new ApplicationUser(userId, email);

        // await _userManager.AddClaimAsync(newUser, new Claim("tags.view", "true"));

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

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        if (expiryDateTimeUtc > now)
            return AuthenticationResult.Fail(IdentityErrors.TokenHasntExpiredYet);

        // TODO Uncomment when ready
        RefreshToken storedRefreshToken = null; //await _authenticationGateWay.GetRefreshToken(request.RefreshToken);
        if (storedRefreshToken is null)
            return AuthenticationResult.Fail(IdentityErrors.TokenDoesNotExist);

        if (now > storedRefreshToken.ExpiryDate)
            return AuthenticationResult.Fail(IdentityErrors.TokenHasExpired);

        if (storedRefreshToken.Invalidated)
            return AuthenticationResult.Fail(IdentityErrors.RefreshTokenIsInvalidated);

        if (storedRefreshToken.Used)
            return AuthenticationResult.Fail(IdentityErrors.RefreshedTokenIsAlreadyUsed);

        var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

        if (storedRefreshToken.JwtId != jti)
            return AuthenticationResult.Fail(IdentityErrors.RefreshTokenDoesNotMatchJWT);

        storedRefreshToken.SetInUse();

        //await _authenticationGateWay.UpdateRefreshToken(storedRefreshToken);

        var userId = validatedToken.Claims.Single(x => x.Type == "id").Value;
        var user = await _userManager.FindByIdAsync(userId);

        return await GenerateAuthenticationResultForUserAsync(user);
    }

    private ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        try
        {
            return GetPrincipal(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Token Validation");

            return null;
        }
    }

    private ClaimsPrincipal GetPrincipal(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);

        return IsJwtWithValidSecurityAlgorithm(validatedToken) ? principal : null;
    }

    private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken) => validatedToken
        is JwtSecurityToken jwtSecurityToken
        && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

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

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var key = Encoding.ASCII.GetBytes(_jwtSettings.CurrentValue.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = now.AddHours(_jwtSettings.CurrentValue.TokenLifetime.Minutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // TODO use token.ValidFrom and ValitTo
        var refreshToken = RefreshToken.New(token.Id, user.Id, now, now.AddHours(2));

        // await _authenticationGateWay.AddRefreshToken(refreshToken);

        var tokenString = tokenHandler.WriteToken(token);

        return AuthenticationResult.Success(tokenString, refreshToken.Token);
    }
}