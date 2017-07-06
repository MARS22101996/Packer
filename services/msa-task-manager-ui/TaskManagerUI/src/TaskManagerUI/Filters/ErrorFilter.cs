using System;
using EpamMA.Communication.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskManagerUI.Filters
{
    public class ErrorFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception;
            if (!filterContext.ExceptionHandled)
            {
                var communicationException = exception as ServiceCommunicationException;

                if (communicationException != null && communicationException.Message.Contains("401"))
                {
                    filterContext.Result = new RedirectToActionResult("Login", "Account", null);
                    filterContext.ExceptionHandled = true;
                }
            }
        }
    }
}