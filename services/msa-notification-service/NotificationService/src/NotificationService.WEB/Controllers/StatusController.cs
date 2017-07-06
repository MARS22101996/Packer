using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NotificationService.WEB.Controllers
{
    [Route("NotificationService/status")]
    public class StatusController : Controller
    {
        /// <summary>
        /// Shows microservice life status
        /// </summary>
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Alive")]
        public StatusCodeResult Get()
        {
            return Ok();
        }
    }
}