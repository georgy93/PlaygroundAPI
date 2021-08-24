namespace Playground.Infrastructure.Identity.Models
{
    public record UserRegistrationRequest
    {
        // TODO Add Tests and fluent validations

        public string Email { get; init; }

        public string Password { get; init; }
    }
}