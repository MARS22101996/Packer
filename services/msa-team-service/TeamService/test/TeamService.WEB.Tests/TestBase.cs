using System;
using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using TeamService.WEB.Infrastructure.Automapper;

namespace TeamService.WEB.Tests
{
    public class TestBase
    {
        protected IMapper GetMapper()
        {
            var config = new AutoMapperConfiguration();

            return config.Configure().CreateMapper();
        }

        protected Mock<ILogger<T>> GetLoggerMock<T>() where T : class
        {
            return new Mock<ILogger<T>>();
        }

        protected void SetupRequestHeader<TEntity>(TEntity controller, Guid userId) where TEntity : Controller
        {
            var headersMock = new HeaderDictionary
           {
               new KeyValuePair<string, StringValues>("Content-Type", "Content-Type_Header_Test_Value"),
               new KeyValuePair<string, StringValues>("Authorization", "Authorization_Header_Test_Value"),
               new KeyValuePair<string, StringValues>("CorrelationId", "CorrelationId_Header_Test_Value")
           };
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();

            httpContextMock
                .Setup(m => m.User)
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", userId.ToString()) })));
            httpContextMock.Setup(m => m.Request).Returns(httpRequestMock.Object);
            httpContextMock.SetupGet(x => x.Request.Headers).Returns(headersMock);

            var actionContext = new ActionContext(
                    httpContextMock.Object,
                    new Mock<RouteData>().Object,
                    new Mock<ActionDescriptor>().Object);

            controller.ControllerContext = new ControllerContext(new ActionContext()
            {
                RouteData = new RouteData(),
                HttpContext = actionContext.HttpContext,
                ActionDescriptor = new ControllerActionDescriptor()
            });
        }
    }
}