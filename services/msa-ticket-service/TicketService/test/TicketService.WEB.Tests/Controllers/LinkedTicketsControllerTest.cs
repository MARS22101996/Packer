using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TicketService.BLL.DTO;
using TicketService.BLL.Interfaces;
using TicketService.Tests.Core.Attributes;
using TicketService.Tests.Core.Enums;
using TicketService.WEB.Controllers;
using Xunit;

namespace TicketService.WEB.Tests.Controllers
{
    [Category(TestType.Unit)]
    public class LinkedTicketsControllerTest : TestBase
    {
        private readonly Mock<ITicketLinkService> _ticketLinkServiceMock;
        private readonly LinkedTicketsController _sut;

        public LinkedTicketsControllerTest()
        {
            SetUp();

            _ticketLinkServiceMock = new Mock<ITicketLinkService>();
            var loggerMock = new Mock<ILogger<LinkedTicketsController>>();

            _sut = new LinkedTicketsController(
                _ticketLinkServiceMock.Object,
                loggerMock.Object,
                Mapper);
        }

        [Fact]
        public async Task GetUnlinkedTickets_ReturnsUnlinkedTicketsInJson_WhenTicketIdValid()
        {
            var ticketDtos = new List<TicketDto>();
            var ticketId = Guid.NewGuid();

            _ticketLinkServiceMock.Setup(method => method.GetUnlinkedTicketsAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(ticketDtos);

            var result = await _sut.GetUnLinkedTickets(It.IsAny<Guid>(), ticketId);
            var jsonResult = result as JsonResult;

            Assert.NotNull(jsonResult?.Value);
        }

        [Fact]
        public async Task GetUnlinkedTickets_ReturnsBadRequest_WhenTicketIdInvalid()
        {
            Guid? ticketId = null;

            var result = await _sut.GetUnLinkedTickets(It.IsAny<Guid>(), ticketId);

            Assert.Equal(typeof(BadRequestResult), result.GetType());
        }

        [Fact]
        public async Task GetLinkedTickets_ReturnsLinkedTicketsInJson_WhenTicketIdValid()
        {
            var ticketId = Guid.NewGuid();
            var ticketDtos = new List<TicketDto>();

            _ticketLinkServiceMock.Setup(method => method.GetLinkedTicketsAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(ticketDtos);

            var result = await _sut.GetUnLinkedTickets(It.IsAny<Guid>(), ticketId);
            var jsonResult = result as JsonResult;

            Assert.NotNull(jsonResult?.Value);
        }

        [Fact]
        public async Task GetLinkedTickets_ReturnsBadRequest_WhenTicketIdInvalid()
        {
            Guid? ticketId = null;

            var result = await _sut.GetLinkedTickets(It.IsAny<Guid>(), ticketId);

            Assert.Equal(typeof(BadRequestResult), result.GetType());
        }

        [Fact]
        public async Task LinkTickets_ReturnsOkResult_WhenSourceAndDistIdsValid()
        {
            var sourceId = Guid.NewGuid();
            var distinationId = Guid.NewGuid();

            var result = await _sut.LinkTickets(It.IsAny<Guid>(), sourceId, distinationId);

            Assert.Equal(typeof(OkResult), result.GetType());
        }

        [Fact]
        public async Task LinkTickets_ReturnsBadRequest_WhenSorceOrDestinationIdsInvalid()
        {
            Guid? sourceId = null;
            Guid? destinationId = null;

            var result = await _sut.LinkTickets(It.IsAny<Guid>(), sourceId, destinationId);

            Assert.Equal(typeof(BadRequestResult), result.GetType());
        }

        [Fact]
        public async Task UnLinkTickets_ReturnsOkResult_WhenSourceAndDestinationIdsValid()
        {
            var sourceId = Guid.NewGuid();
            var destinationId = Guid.NewGuid();

            var result = await _sut.UnLinkTickets(It.IsAny<Guid>(), sourceId, destinationId);

            Assert.Equal(typeof(OkResult), result.GetType());
        }

        [Fact]
        public async Task UnLinkTickets_ReturnsBadRequest_WhenSorceOrDestinationIdsInvalid()
        {
            Guid? sourceId = null;
            Guid? destinationId = null;

            var result = await _sut.UnLinkTickets(It.IsAny<Guid>(), sourceId, destinationId);

            Assert.Equal(typeof(BadRequestResult), result.GetType());
        }
    }
}