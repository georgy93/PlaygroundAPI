namespace Playground.Application.Interfaces.Gateways;

using Refit;

public interface ISomeServiceGateway
{
    //[Get("/someController/someEndpoint")]
    [Get("/")]
    Task<string> GetInfoAsync(CancellationToken cancellationToken);
}