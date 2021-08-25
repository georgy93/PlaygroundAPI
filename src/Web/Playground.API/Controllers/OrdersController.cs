namespace Playground.API.Controllers
{
    using Application.Queries.GetOrderByIdQuery;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Net;
    using System.Threading.Tasks;

    public class OrdersController : BaseController
    {
        [HttpGet("{orderId:int}")]
        [ProducesResponseType(typeof(GetOrderByIdQueryResult), StatusCodes.Status200OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<GetOrderByIdQueryResult> GetOrderAsync(int orderId)
        {
            return await Mediator.Send(new GetOrderByIdQuery(orderId));
        }
    }
}