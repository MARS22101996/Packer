using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using StatisticService.WEB.Infrastructure.Automapper;

namespace StatisticService.WEB.Tests
{
    public class TestBase
    {
        protected IMapper Mapper { get; set; }

        protected void SetUp()
        {
            var config = new AutoMapperConfiguration();
            Mapper = config.Configure().CreateMapper();
        }

        protected void SetupRequestHeader<T>(T sut) where T : Controller
        {
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            httpContextMock.Setup(m => m.Request).Returns(httpRequestMock.Object);

            var actionContext = new ActionContext(
                    httpContextMock.Object,
                    new Mock<RouteData>().Object,
                    new Mock<ActionDescriptor>().Object);

            sut.ControllerContext = new ControllerContext(new ActionContext()
            {
                RouteData = new RouteData(),
                HttpContext = actionContext.HttpContext,
                ActionDescriptor = new ControllerActionDescriptor()
            });
        }
    }
}