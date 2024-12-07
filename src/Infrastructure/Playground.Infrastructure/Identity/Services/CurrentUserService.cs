namespace Playground.Infrastructure.Identity.Services;

using Domain.Services;
using Microsoft.AspNetCore.Http;

internal class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string UserId { get; } = httpContextAccessor
            .HttpContext
            .User
            .Claims
            .Single(c => c.Type == "id")?
            .Value ?? string.Empty;
}