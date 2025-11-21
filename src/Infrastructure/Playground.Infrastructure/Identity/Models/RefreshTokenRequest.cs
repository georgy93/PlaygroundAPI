namespace Playground.Infrastructure.Identity.Models;

public record RefreshTokenRequest
{
    public string Token { get; init; }
}