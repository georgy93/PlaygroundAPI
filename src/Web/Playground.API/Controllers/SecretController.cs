namespace Playground.API.Controllers
{
    using Behavior.Filters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using System.Threading.Tasks;

    [Route("secret")]
    [ApiKeyAuth]
    [AllowAnonymous]
    public class SecretController : BaseController
    {
        public SecretController(/*IMapper mapper*/)
        //: base(mapper)
        {
        }

        [HttpGet("get")]
        public IActionResult GetSecret() => Ok("I have no secrets");

        [HttpGet("ct")]
        public async Task MapRequestAbortedToCancellationTokenParameter(CancellationToken cancellationToken)
        {
            await Task.Delay(10000, cancellationToken);
        }
    }
}