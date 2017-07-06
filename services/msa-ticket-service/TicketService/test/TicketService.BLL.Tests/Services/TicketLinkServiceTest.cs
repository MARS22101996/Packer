using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TicketService.BLL.Infrastructure.Exceptions;
using TicketService.BLL.Interfaces;
using TicketService.BLL.Services;
using TicketService.DAL.Entities;
using TicketService.DAL.Interfaces;
using TicketService.Tests.Core.Attributes;
using TicketService.Tests.Core.Enums;

namespace TicketService.BLL.Tests.Services
{
    [Category(TestType.Unit)]
    public class TicketLinkServiceTest : TestBase
    {
        private readonly ITicketLinkService _sut;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public TicketLinkServiceTest()
        {
            SetUp();
            var mockLog = new Mock<ILogger<TicketLinkService>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new TicketLinkService(_unitOfWorkMock.Object, Mapper, mockLog.Object);
        }

        [Fact]
        public async Task GetLinkedTickets_ReturnsLinkedTickets_IfTheyExist()
        {
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tickets.FindAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Ticket, bool>>>()))
                .ReturnsAsync(new List<Ticket>());

            await _sut.GetLinkedTicketsAsync(Guid.NewGuid(), Guid.NewGuid());

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.Tickets.FindAsync(
                It.IsAny<Guid>(),
                It.IsAny<Expression<Func<Ticket, bool>>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetUnlinkedTickets_ReturnsUnlinkedTickets_IfTheyExist()
        {
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tickets.FindAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Ticket, bool>>>()))
                .ReturnsAsync(new List<Ticket>());

            await _sut.GetUnlinkedTicketsAsync(It.IsAny<Guid>(), Guid.NewGuid());

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.Tickets.FindAsync(
                It.IsAny<Guid>(),
                It.IsAny<Expression<Func<Ticket, bool>>>()),
                Times.Once);
        }

        [Fact]
        public async Task LinkTickets_UpdatesBothTickets_IfTheyExist()
        {
            var stubTicketId1 = Guid.NewGuid();
            var stubTicketId2 = Guid.NewGuid();

            var stubTickets = new List<Ticket>
            {
                new Ticket { Id = stubTicketId1 },
                new Ticket { Id = stubTicketId2 }
            };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tickets.FindAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Ticket, bool>>>()))
                .ReturnsAsync(stubTickets);

            await _sut.LinkTicketsAsync(It.IsAny<Guid>(), stubTicketId1, stubTicketId2);

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.Tickets.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Ticket>()), Times.Exactly(2));
        }

        [Fact]
        public async Task UnlinkTickets_UpdatesBothTickets_IfTheyExist()
        {
            var stubTicketId1 = Guid.NewGuid();
            var stubTicketId2 = Guid.NewGuid();

            var stubTickets = new List<Ticket>
            {
                new Ticket { Id = stubTicketId1 },
                new Ticket { Id = stubTicketId2 }
            };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tickets.FindAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Ticket, bool>>>()))
                .ReturnsAsync(stubTickets.AsQueryable());

            await _sut.UnlinkTicketsAsync(It.IsAny<Guid>(), stubTicketId1, stubTicketId2);

            _unitOfWorkMock.Verify(x => x.Tickets.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Ticket>()), Times.Exactly(2));
        }

        [Fact]
        public async Task UnlinkTickets_ThrowsEntityNotFoundException_IfTicketDoesNotExist()
        {
            var stubTicketId1 = Guid.NewGuid();
            var stubTicketId2 = Guid.NewGuid();

            var stubTicket1 = new Ticket { Id = stubTicketId1 };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tickets.GetAsync(It.IsAny<Guid>(), stubTicketId1))
                .ReturnsAsync(stubTicket1);

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tickets.GetAsync(It.IsAny<Guid>(), stubTicketId2))
                .ReturnsAsync(null);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _sut.UnlinkTicketsAsync(It.IsAny<Guid>(), stubTicketId1, stubTicketId2));
        }
    }
}