namespace Playground.API.Controllers;

using Application.Queries.GetOrderByIdQuery;
using System.Net;

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