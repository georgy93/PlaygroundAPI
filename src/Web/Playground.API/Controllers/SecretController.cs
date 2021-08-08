namespace Playground.API.Controllers
{
    using Behavior.Filters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using System.Threading.Tasks;

    [ApiKeyAuth]
    [AllowAnonymous]
    public class SecretController : BaseController
    {
        public SecretController(/*IMapper mapper*/)
        //: base(mapper)
        {
        }

        [HttpGet(ApiRoutes.Secret.Get)]
        public IActionResult GetSecret() => Ok("I have no secrets");

        // TODO: Move this
        [HttpGet(ApiRoutes.Secret.CancellationTokenMap)]
        public async Task MapRequestAbortedToCancellationTokenParameterAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(10000, cancellationToken);
        }
    }
}