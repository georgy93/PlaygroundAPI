namespace Playground.Infrastructure.Identity.Models
{
    public record UserLoginRequest
    {
        public string Email { get; init; }

        public string Password { get; init; }
    }
}