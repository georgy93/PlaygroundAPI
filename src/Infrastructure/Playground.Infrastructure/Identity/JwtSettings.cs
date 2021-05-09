namespace Playground.Infrastructure.Identity
{
    using System;

    public class JwtSettings
    {
        public string Secret { get; set; }

        public TimeSpan TokenLifetime { get; set; }
    }
}