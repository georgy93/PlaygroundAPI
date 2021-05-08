namespace Playground.Infrastructure.Identity.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public record AuthFailedResponse(IEnumerable<string> Errors = null)
    {
        public IEnumerable<string> Errors { get; init; } = Errors ?? Enumerable.Empty<string>();
    }
}