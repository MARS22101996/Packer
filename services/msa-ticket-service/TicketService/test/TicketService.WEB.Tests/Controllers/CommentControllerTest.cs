using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using TicketService.BLL.DTO;
using TicketService.BLL.Interfaces;
using TicketService.Tests.Core.Attributes;
using TicketService.Tests.Core.Enums;
using TicketService.WEB.Controllers;
using TicketService.WEB.Models;
using Xunit;
using EpamMA.Communication.Interfaces;

namespace TicketService.WEB.Tests.Controllers
{
    [Category(TestType.Unit)]
    public class CommentControllerTest : TestBase
    {
        private const string ContentTypeHeaderKey = "Content-Type";
        private const string AuthorizationHeaderKey = "Authorization";
        private const string CorrelationIdHeaderKey = "CorrelationId";
        private readonly Mock<ICommentService> _commentServiceMock;
        private readonly Mock<ICommunicationService> _communicationServiceMock;
        private readonly CommentsController _sut;

        public CommentControllerTest()
        {
            SetUp();

            _communicationServiceMock = new Mock<ICommunicationService>();
            _commentServiceMock = new Mock<ICommentService>();
            var loggerMock = new Mock<ILogger<CommentsController>>();

            _sut = new CommentsController(
                Mapper,
                _commentServiceMock.Object,
                loggerMock.Object,
                _communicationServiceMock.Object);
        }

        [Fact]
        public async Task Post_ReturnsOk_WhenValidCommentViewModel()
        {
            var httpContextMock = new Mock<HttpContext>();
            var model = new CommentApiModel();
            var ticketId = Guid.NewGuid();
            var mockPrincipal = new Mock<ClaimsPrincipal>();

            mockPrincipal.Setup(x => x.FindFirst(It.IsAny<string>())).Returns(new Claim(string.Empty, string.Empty));
            httpContextMock.Setup(m => m.User).Returns(mockPrincipal.Object);

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

            _communicationServiceMock.Setup(
                method => method.GetAsync<UserApiModel>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
                .ReturnsAsync(new UserApiModel());

            var result = await _sut.Post(It.IsAny<Guid>(), ticketId, model);

            Assert.Equal(typeof(OkObjectResult), result.GetType());
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenInValidCommentViewModel()
        {
            var model = new CommentApiModel();
            var ticketId = Guid.NewGuid();

            _sut.ModelState.AddModelError("Model", "Invalid");

            var result = await _sut.Post(It.IsAny<Guid>(), ticketId, model);

            Assert.Equal(typeof(BadRequestResult), result.GetType());
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenCommentExists()
        {
            var commentId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();

            var result = await _sut.Delete(It.IsAny<Guid>(), commentId, ticketId);

            Assert.Equal(typeof(OkResult), result.GetType());
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenCommentExists()
        {
            var result = await _sut.Delete(It.IsAny<Guid>(), null, null);

            Assert.Equal(typeof(BadRequestResult), result.GetType());
        }

        [Fact]
        public async Task Get_ReturnsCommentApiModelsInJson_WhenCommentsExists()
        {
            var id = Guid.NewGuid();
            var commentDtos = new List<CommentDto>();

            _commentServiceMock.Setup(method => method.GetAllAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(commentDtos);

            var result = await _sut.Get(It.IsAny<Guid>(), id);
            var jsonResult = result as JsonResult;

            Assert.NotNull(jsonResult?.Value);
        }
    }
}
