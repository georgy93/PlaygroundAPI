namespace Playground.Infrastructure.Messaging.Rest.DelegatingHandlers
{
    using Microsoft.Extensions.Logging;
    using System.Net.Http;
    using Utils.Extensions;

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
                return await base.SendAsync(request, cancellationToken);  // await so that exceptions are unwrapped
            }
            catch (Exception ex)
            {
                var logData = new
                {
                    HttpMethod = request.Method,
                    Uri = request.RequestUri,
                    RequestPayload = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken)
                };

                _logger.LogError(ex, logData.Beautify());

                // TODO: throw new ServiceNotAvailableException(request.RequestUri);
                throw new Exception("error", ex);
            }
        }
    }
}