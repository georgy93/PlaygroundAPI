namespace Playground.Application.Interfaces.Gateways
{
    using Refit;
    using System.Threading.Tasks;

    public interface ISomeServiceGateway
    {
        //[Get("/someController/someEndpoint")]
        [Get("/")]
        Task<string> GetInfoAsync();
    }
}