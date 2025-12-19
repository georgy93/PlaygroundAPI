namespace Playground.Utils.Extensions;

using Microsoft.AspNetCore.Http;
using System.Text;

// TODO use the better method
public static class HttpContextExtensions
{
    extension(HttpContext context)
    {
        public string GenerateCacheKeyFromRequest()
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append($"{context.Request.Path}");

            foreach (var (key, value) in context.Request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}