using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json.Linq;
using TicketService.DAL.Context;
using TicketService.DAL.Interfaces;
using TicketService.DAL.UnitOfWorks;
using TicketService.WEB.Infrastructure.AutoMapper;

namespace TicketService.IntegrationTests
{
    public class TestBase : IDisposable
    {
        protected readonly Guid StubTeamId = Guid.Parse("54879340-95bc-46cb-b733-4f7434d3d2f9");
        private const string DbConfigPath = "bin\\Debug\\netcoreapp1.0\\config.json";
        private const string DbConfigSection = "connectionString";
        private const string IdentityUserMockName = "test_name";

        private const string ContentTypeHeaderKey = "Content-Type";
        private const string AuthorizationHeaderKey = "Authorization";
        private const string CorrelationIdHeaderKey = "CorrelationId";

        protected readonly IUnitOfWork UnitOfWork;

        protected IMapper Mapper { get; private set; }

        protected TestBase()
        {
            UnitOfWork = GetUnitOfWork();
        }

        public virtual void Dispose()
        {
        }
         
        protected void MapperSetUp()
        {
            var config = new AutoMapperConfiguration();
            Mapper = config.Configure().CreateMapper();
        }

        protected void SetupRequestHeader<TEntity>(TEntity controller) where TEntity : Controller
        {
            var headersMock = new HeaderDictionary
            {
                new KeyValuePair<string, StringValues>(ContentTypeHeaderKey, "Content-Type_Header_Test_Value"),
                new KeyValuePair<string, StringValues>(AuthorizationHeaderKey, "Authorization_Header_Test_Value"),
                new KeyValuePair<string, StringValues>(CorrelationIdHeaderKey, "CorrelationId_Header_Test_Value")
            };
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            var mockIdentity = new Mock<IIdentity>();
            httpContextMock.Setup(m => m.Request).Returns(httpRequestMock.Object);
            httpContextMock.SetupGet(x => x.User.Identity).Returns(mockIdentity.Object);
            httpContextMock.SetupGet(x => x.Request.Headers).Returns(headersMock);
            mockIdentity.Setup(x => x.Name).Returns(IdentityUserMockName);

            var actionContext = new ActionContext(
                    httpContextMock.Object,
                    new Mock<RouteData>().Object,
                    new Mock<ActionDescriptor>().Object);

            controller.ControllerContext = new ControllerContext(new ActionContext
            {
                RouteData = new RouteData(),
                HttpContext = actionContext.HttpContext,
                ActionDescriptor = new ControllerActionDescriptor()
            });
        }

        protected Mock<ILogger<T>> GetLoggerMock<T>() where T : class
        {
            return new Mock<ILogger<T>>();
        }

        private UnitOfWork GetUnitOfWork()
        {
            var test = Path.Combine(Directory.GetCurrentDirectory(), DbConfigPath);
            var configJsonString = File.ReadAllText(test);
            var parsedJsonString = JObject.Parse(configJsonString);
            var connectionString = parsedJsonString[DbConfigSection];

            var databaseContext = new DbContext(connectionString.ToString());
            var unitOfWork = new UnitOfWork(databaseContext);

            return unitOfWork;
        }
    }
}