using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TicketService.BLL.DTO;
using TicketService.BLL.Interfaces;
using TicketService.Tests.Core.Attributes;
using TicketService.Tests.Core.Enums;
using TicketService.WEB.Controllers;
using TicketService.WEB.Models;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using EpamMA.Communication.Interfaces;

namespace TicketService.WEB.Tests.Controllers
{
    [Category(TestType.Unit)]
    public class TicketControllerTest : TestBase
    {
        private const string ContentTypeHeaderKey = "Content-Type";
        private const string AuthorizationHeaderKey = "Authorization";
        private const string CorrelationIdHeaderKey = "CorrelationId";
        private readonly Mock<ITicketService> _ticketServiceMock;
        private readonly Mock<ICommunicationService> _communicationServiceMock;
        private readonly Mock<ILogger<TicketsController>> _loggerMock;
        private readonly TicketsController _sut;

        public TicketControllerTest()
        {
            SetUp();

            _ticketServiceMock = new Mock<ITicketService>();
            _communicationServiceMock = new Mock<ICommunicationService>();
            _loggerMock = new Mock<ILogger<TicketsController>>();

            _sut = new TicketsController(
                Mapper,
                _ticketServiceMock.Object,
                _communicationServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenValidTicketViewModel()
        {
            var model = new TicketApiModel();
            _sut.ModelState.AddModelError("Model", "Model is not valid");

            var result = await _sut.Create(It.IsAny<Guid>(), model);

            Assert.Equal(typeof(BadRequestObjectResult), result.GetType());
        }

        [Fact]
        public async Task Create_ReturnsStatusOk_WhenValidTicketViewModel()
        {
            var httpContextMock = new Mock<HttpContext>();

            var headersMock = new HeaderDictionary
           {
               new KeyValuePair<string, StringValues>(ContentTypeHeaderKey, "Content-Type_Header_Test_Value"),
               new KeyValuePair<string, StringValues>(AuthorizationHeaderKey, "Authorization_Header_Test_Value"),
               new KeyValuePair<string, StringValues>(CorrelationIdHeaderKey, "CorrelationId_Header_Test_Value")
           };

            httpContextMock.SetupGet(x => x.Request.Headers).Returns(headersMock);

            var actionContext = new ActionContext(
                        httpContextMock.Object,
                        new Mock<RouteData>().Object,
                        new Mock<ActionDescriptor>().Object);

            _sut.ControllerContext = new ControllerContext(new ActionContext()
            {
                RouteData = new RouteData(),
                HttpContext = actionContext.HttpContext,
                ActionDescriptor = new ControllerActionDescriptor()
            });

            var ticketApiModel = new TicketApiModel();

            _ticketServiceMock
                .Setup(ticket => ticket.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new TicketDto());

            var result = await _sut.Create(It.IsAny<Guid>(), ticketApiModel);

            Assert.Equal(typeof(OkObjectResult), result.GetType());
        }

        [Fact]
        public async Task Edit_ReturnsStatusOk_WhenCommunicationToUserSuccess()
        {
            var ticketId = Guid.NewGuid();
            var ticketDto = new TicketDto { Id = ticketId };
            var model = new TicketApiModel();

            var httpContextMock = new Mock<HttpContext>();

            var headersMock = new HeaderDictionary
           {
               new KeyValuePair<string, StringValues>(ContentTypeHeaderKey, "Content-Type_Header_Test_Value"),
               new KeyValuePair<string, StringValues>(AuthorizationHeaderKey, "Authorization_Header_Test_Value"),
               new KeyValuePair<string, StringValues>(CorrelationIdHeaderKey, "CorrelationId_Header_Test_Value")
           };

            httpContextMock.SetupGet(x => x.Request.Headers).Returns(headersMock);

            var actionContext = new ActionContext(
                        httpContextMock.Object,
                        new Mock<RouteData>().Object,
                        new Mock<ActionDescriptor>().Object);

            _sut.ControllerContext = new ControllerContext(new ActionContext()
            {
                RouteData = new RouteData(),
                HttpContext = actionContext.HttpContext,
                ActionDescriptor = new ControllerActionDescriptor()
            });

            _ticketServiceMock
                .Setup(manager => manager.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(ticketDto);

            _communicationServiceMock
                .Setup(method => method.GetAsync<HttpStatusCode>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<HeaderDictionary>(), It.IsAny<string>()))
                .ReturnsAsync(HttpStatusCode.OK);

            var result = await _sut.Update(It.IsAny<Guid>(), model);

            Assert.Equal(typeof(OkResult), result.GetType());
        }

        [Fact]
        public async Task Delete_ReturnsStatusOk_WhenTicketIsExicted()
        {
            var ticketId = Guid.NewGuid();
            var model = new TicketDto { Id = ticketId };

            var httpContextMock = new Mock<HttpContext>();

            var headersMock = new HeaderDictionary
           {
               new KeyValuePair<string, StringValues>(ContentTypeHeaderKey, "Content-Type_Header_Test_Value"),
               new KeyValuePair<string, StringValues>(AuthorizationHeaderKey, "Authorization_Header_Test_Value"),
               new KeyValuePair<string, StringValues>(CorrelationIdHeaderKey, "CorrelationId_Header_Test_Value")
           };

            httpContextMock.SetupGet(x => x.Request.Headers).Returns(headersMock);

            var actionContext = new ActionContext(
                        httpContextMock.Object,
                        new Mock<RouteData>().Object,
                        new Mock<ActionDescriptor>().Object);

            _sut.ControllerContext = new ControllerContext(new ActionContext()
            {
                RouteData = new RouteData(),
                HttpContext = actionContext.HttpContext,
                ActionDescriptor = new ControllerActionDescriptor()
            });

            _ticketServiceMock
                .Setup(manager => manager.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(model);

            var result = await _sut.Delete(It.IsAny<Guid>(), ticketId);

            Assert.Equal(typeof(OkResult), result.GetType());
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenNullInput()
        {
            var result = await _sut.Delete(It.IsAny<Guid>(), null);

            Assert.Equal(typeof(BadRequestResult), result.GetType());
        }

        [Fact]
        public async Task GetTicket_ReturnsViewWithTicketAndComments_IfIdIsValid()
        {
            var guid = Guid.NewGuid();
            _ticketServiceMock
                .Setup(m => m.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new TicketDto());

            var result = await _sut.GetTicket(It.IsAny<Guid>(), guid);
            var jsonResult = result as JsonResult;

            Assert.NotNull(jsonResult?.Value);
        }
    }
}