namespace Playground.Infrastructure.Identity.Models
{
    public record UserRegistrationRequest
    {
        public string Email { get; init; }

        public string Password { get; init; }
    }
}