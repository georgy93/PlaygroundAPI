namespace Playground.Infrastructure.RestMessaging.DelegatingHandlers
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    internal class RequestExceptionHandlingBehavior : DelegatingHandler
    {
        private readonly ILogger<RequestExceptionHandlingBehavior> _logger;

        public RequestExceptionHandlingBehavior(ILogger<RequestExceptionHandlingBehavior> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                // await so that exceptions are unwrapped
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                var log = JsonSerializer.Serialize(new
                {
                    Uri = request.RequestUri,
                    HttpMethod = request.Method,
                    Body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken)
                },
                new JsonSerializerOptions() { WriteIndented = true });

                _logger.LogError(ex, log);

                // TODO: throw new ServiceNotAvailableException(request.RequestUri);
                throw new Exception();
            }
        }
    }
}