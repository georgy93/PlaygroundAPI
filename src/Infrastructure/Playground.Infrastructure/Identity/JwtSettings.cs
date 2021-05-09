namespace Playground.Infrastructure.Identity
{
    using System;

    internal class JwtSettings
    {
        public string Secret { get; set; }

        public TimeSpan TokenLifetime { get; set; }
    }
}