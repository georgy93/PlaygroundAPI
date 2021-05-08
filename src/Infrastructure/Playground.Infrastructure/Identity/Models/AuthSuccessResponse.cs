namespace Playground.Infrastructure.Identity.Models
{
    public record AuthSuccessResponse(string Token, string RefreshToken);
}