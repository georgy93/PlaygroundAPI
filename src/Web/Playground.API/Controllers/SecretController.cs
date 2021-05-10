namespace Playground.API.Controllers
{
    using Behavior.Filters;
    using Microsoft.AspNetCore.Mvc;

    [Route("secret")]
    [ApiKeyAuth]
    public class SecretController : BaseController
    {
        public SecretController(/*IMapper mapper*/)
            //: base(mapper)
        {
        }

        [HttpGet("secret")]
        public IActionResult GetSecret()
        {
            return Ok(/*BaseResponse("I have no secrets")*/);
        }
    }
}