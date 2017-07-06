using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using StatisticService.WEB.Models;
using EpamMA.Communication.Infrastructure.Exceptions;

namespace StatisticService.WEB.Filters
{
    public class ErrorFilter : Attribute, IExceptionFilter
    {
        private readonly ILogger<ErrorFilter> _logger;

        public ErrorFilter(ILogger<ErrorFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext filterContext)
        {
            _logger.LogError(
                $"Unhandled exception: {0} | StackTrace: {1} ",
               filterContext.Exception.Message,
               filterContext.Exception.StackTrace);
            if (!filterContext.ExceptionHandled)
            {
                if (filterContext.Exception is ServiceCommunicationException)
                {
                    filterContext.Result = new BadRequestObjectResult(new HandleErrorInfo
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        StackTrace = filterContext.Exception.StackTrace
                    });
                }

                filterContext.ExceptionHandled = true;
            }
        }
    }
}