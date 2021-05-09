namespace Playground.Infrastructure.Identity.Services
{
    using Application.Interfaces;
    using Microsoft.AspNetCore.Http;
    using System.Linq;

    internal class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor) => UserId = 
            httpContextAccessor
            .HttpContext
            .User
            .Claims
            .Single(c => c.Type == "id")?
            .Value ?? string.Empty;

        public string UserId { get; }
    }
}