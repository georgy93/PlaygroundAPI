namespace Playground.Infrastructure.Messaging.Rest.DelegatingHandlers
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    internal class RequestStatisticsBehavior : DelegatingHandler
    {
        //private readonly IMetricsRoot _metrics;

        //public RequestStatisticsBehavior(IMetricsRoot metrics)
        //{
        //    _metrics = metrics;
        //}

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //using (_metrics.Measure.Timer.Time(null))
            // {
            // await so that exceptions are unwrapped
            return await base.SendAsync(request, cancellationToken);
            // }
        }
    }
}