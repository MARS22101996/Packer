using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using TeamService.BLL.Infrastructure.Exceptions;
using TeamService.WEB.Infrastructure;

namespace TeamService.WEB.Controllers
{
    public class BaseController : Controller
    {
        protected Guid CurrentUserId
        {
            get
            {
                var user = User.GetUserId();

                if (!user.HasValue)
                {
                    throw new ServiceException("Cannot receive current user id from token", "User");
                }

                return user.Value;
            }
        }

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