namespace Playground.API.Controllers;

using DTOs;
using MediatR;
using System.Net.Mime;

[ApiController] // this will return automatically error response when model state is invalid
                // We can tackle it by adding this configuration
                // services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
[Produces(MediaTypeNames.Application.Json)]
[ProducesErrorResponseType(typeof(ErrorResponse))]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public abstract class BaseController : ControllerBase
{
    protected IMediator Mediator => HttpContext.Features.Get<IMediator>();
}