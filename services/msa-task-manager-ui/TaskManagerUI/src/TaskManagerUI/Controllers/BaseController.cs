﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace TaskManagerUI.Controllers
{
    public class BaseController : Controller
    {
        protected const string JsonType = "application/json";
        protected const string FormType = "application/x-www-form-urlencoded";

        protected IHeaderDictionary FormHeaders(string contentType)
        {
            var headers = new HeaderDictionary
            {
                new KeyValuePair<string, StringValues>("Content-Type", new StringValues(contentType))
            };

            if (Request.Headers.ContainsKey("Authorization"))
            {
                headers
                    .Add(new KeyValuePair<string, StringValues>("Authorization", Request.Headers["Authorization"]));
            }

            if (Request.Headers.ContainsKey("CorrelationId"))
            {
                headers
                    .Add(new KeyValuePair<string, StringValues>("CorrelationId", Request.Headers["CorrelationId"]));
            }

            return headers;
        }
    }
}