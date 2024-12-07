namespace Playground.Infrastructure.Identity;

internal record JwtSettings
{
    public string Secret { get; init; }

    public TimeSpan TokenLifetime { get; init; }
}