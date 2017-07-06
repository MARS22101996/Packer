using System;
using EpamMA.Communication.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using TicketService.BLL.Infrastructure.Exceptions;
using TicketService.Core.Enums;
using TicketService.WEB.Models;

namespace TicketService.WEB.Filters
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
                "Unhandled exception: {0} | StackTrace: {1} ",
                 filterContext.Exception.Message,
                 filterContext.Exception.StackTrace);
            if (!filterContext.ExceptionHandled)
            {
                if (filterContext.Exception is EntityNotFoundException)
                {
                    filterContext.Result = new NotFoundObjectResult(new HandleErrorInfo
                    {
                        StatusCode = (int)ErrorType.HttpNotFound,
                        StackTrace = filterContext.Exception.StackTrace
                    });
                }

                if (filterContext.Exception is ServiceCommunicationException)
                {
                    filterContext.Result = new BadRequestObjectResult(new HandleErrorInfo
                    {
                        StatusCode = (int)ErrorType.BadRequest,
                        StackTrace = filterContext.Exception.StackTrace
                    });
                }

                if (filterContext.Exception is ServiceException)
                {
                    filterContext.Result = new BadRequestObjectResult(new HandleErrorInfo
                    {
                        StatusCode = (int)ErrorType.InternalError,
                        StackTrace = filterContext.Exception.StackTrace
                    });
                }

                filterContext.ExceptionHandled = true;
            }
        }
    }
}