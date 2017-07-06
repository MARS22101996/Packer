using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace TicketService.WEB.Controllers
{
    public class BaseController : Controller
    {
        protected IHeaderDictionary FormHeaders(string contentType)
        {
            var headers = new HeaderDictionary
            {
                new KeyValuePair<string, StringValues>("Content-Type", new StringValues(contentType)),
                new KeyValuePair<string, StringValues>("Authorization", Request.Headers["Authorization"]),
                new KeyValuePair<string, StringValues>("CorrelationId", Request.Headers["CorrelationId"])
            };

            return headers;
        }
    }
}