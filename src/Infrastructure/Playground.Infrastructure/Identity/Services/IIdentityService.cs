namespace Playground.Infrastructure.Identity.Services
{
    using Models;

    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(UserRegistrationRequest request);

        Task<AuthenticationResult> LoginAsync(UserLoginRequest request);

        Task<AuthenticationResult> RefreshTokenAsync(RefreshTokenRequest request);
    }
}