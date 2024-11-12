namespace Playground.Infrastructure.Identity.Models
{
    public record AuthFailedResponse(IEnumerable<string> Errors = null)
    {
        public IEnumerable<string> Errors { get; init; } = Errors ?? [];
    }
}