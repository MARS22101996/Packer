using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UserService.WEB.Controllers
{
    [Route("UserService/status")]
    public class StatusController : Controller
    {
        /// <summary>
        /// Shows microservice life status
        /// </summary>
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Alive")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}