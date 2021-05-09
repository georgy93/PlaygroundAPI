namespace Playground.Infrastructure.Identity.Services
{
    using Models;
    using System.Threading.Tasks;

    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(UserRegistrationRequest request);

        Task<AuthenticationResult> LoginAsync(UserLoginRequest request);

        Task<AuthenticationResult> RefreshTokenAsync(RefreshTokenRequest request);
    }
}