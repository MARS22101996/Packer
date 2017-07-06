using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [Route("status")]
    public class StatusController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
