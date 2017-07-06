using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using TicketService.WEB.Infrastructure.AutoMapper;

namespace TicketService.WEB.Tests
{
    public class TestBase
    {
        protected IMapper Mapper { get; set; }

        public void SetupRequestHeader<TEntity>(TEntity controller) where TEntity : Controller
        {
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            httpContextMock.Setup(m => m.Request).Returns(httpRequestMock.Object);

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

        protected void SetUp()
        {
            var config = new AutoMapperConfiguration();
            Mapper = config.Configure().CreateMapper();
        }
    }
}