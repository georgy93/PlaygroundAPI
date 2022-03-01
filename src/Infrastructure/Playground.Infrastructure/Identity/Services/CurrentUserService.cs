namespace Playground.Infrastructure.Identity.Services
{
    using Domain.Services;
    using Microsoft.AspNetCore.Http;

    internal class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor
                .HttpContext
                .User
                .Claims
                .Single(c => c.Type == "id")?
                .Value ?? string.Empty;
        }

        public string UserId { get; }
    }
}