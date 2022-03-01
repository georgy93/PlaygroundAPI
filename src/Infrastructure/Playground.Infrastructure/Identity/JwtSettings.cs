namespace Playground.Infrastructure.Identity
{
    internal class JwtSettings
    {
        public string Secret { get; set; }

        public TimeSpan TokenLifetime { get; set; }
    }
}