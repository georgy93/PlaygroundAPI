namespace Playground.Infrastructure.Identity
{
    using System;

    public class JwtConfiguration
    {
        public string Secret { get; set; }

        public TimeSpan TokenLifetime { get; set; }
    }
}