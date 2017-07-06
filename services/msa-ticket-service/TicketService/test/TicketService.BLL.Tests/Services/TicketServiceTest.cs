using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using TicketService.BLL.DTO;
using TicketService.BLL.Infrastructure.Exceptions;
using TicketService.BLL.Interfaces;
using TicketService.Core.Enums;
using TicketService.DAL.Entities;
using TicketService.DAL.Interfaces;
using TicketService.Tests.Core.Attributes;
using TicketService.Tests.Core.Enums;
using Xunit;

namespace TicketService.BLL.Tests.Services
{
    [Category(TestType.Unit)]
    public class TicketServiceTest : TestBase
    {
        private readonly ITicketService _sut;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITicketLinkService> _ticketLinkServiceMock;

        public TicketServiceTest()
        {
            SetUp();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            var tagServiceMock = new Mock<ITagService>();
            _ticketLinkServiceMock = new Mock<ITicketLinkService>();
            var mockLog = new Mock<ILogger<BLL.Services.TicketService>>();
            _sut = new BLL.Services.TicketService(
                _unitOfWorkMock.Object,
                Mapper,
                tagServiceMock.Object, 
                _ticketLinkServiceMock.Object, 
                mockLog.Object);
        }

        [Fact]
        public async Task GetAllFiltered_ReturnsTickets()
        {
            var tickets = new List<TicketDto>();
            var filterDto = new FilterDto
            {
                SelectedPriorities = new List<Priority>(),
                SelectedStatuses = new List<Status>()
            };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tickets.FindAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Ticket, bool>>>()))
                .ReturnsAsync(new List<Ticket>());

            var result = await _sut.GetAllFilteredAsync(It.IsAny<Guid>(), filterDto);

            Assert.Equal(tickets.Count, result.Count());
        }

        [Fact]
        public async Task GetAsync_ThrowsEntityNotFoundException_IfTicketIsAbsent()
        {
            var ticketId = Guid.NewGuid();

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tickets.GetAsync(It.IsAny<Guid>(), ticketId))
                .ReturnsAsync(null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.GetAsync(It.IsAny<Guid>(), ticketId));
        }

        [Fact]
        public async Task GetAsync_ThrowsException_WhenTicketIsNotExisted()
        {
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tickets.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(null);
            
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.GetAsync(It.IsAny<Guid>(), Guid.NewGuid()));
        }

        [Fact]
        public async Task CreateAsync_ThrowsException_WhenInputIsNull()
        {
            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Tickets.CreateAsync(It.IsAny<Guid>(), It.IsAny<Ticket>()));
            
            await Assert.ThrowsAsync<NullReferenceException>(() => _sut.CreateAsync(It.IsAny<Guid>(), null));
        }

        [Fact]
        public async Task CreateAsync_CallsCreateAsyncMethod_WhenValidTicket()
        {
            var model = new TicketDto();
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tickets.CreateAsync(It.IsAny<Guid>(), It.IsAny<Ticket>()))
                .ReturnsAsync(Guid.NewGuid());

            await _sut.CreateAsync(It.IsAny<Guid>(), model);

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.Tickets.CreateAsync(It.IsAny<Guid>(), It.IsAny<Ticket>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CallsUpdateAsyncMethod_WhenValidTicket()
        {
            var model = new TicketDto();
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tickets.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Ticket>()))
                .ReturnsAsync(Guid.NewGuid());

            await _sut.UpdateAsync(It.IsAny<Guid>(), model);

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.Tickets.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Ticket>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsException_WhenInputIsNull()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => _sut.CreateAsync(It.IsAny<Guid>(), null));
        }

        [Fact]
        public async Task DeleteAsync_ThrowsException_WhenInputIsNull()
        {
            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Tickets.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));
          
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.DeleteAsync(It.IsAny<Guid>(), Guid.Empty));
        }

        [Fact]
        public async Task DeleteAsync_CallsDeleteMethod_WhenTicketExists()
        {
            var ticketId = Guid.NewGuid();

            _unitOfWorkMock
                .Setup(x => x.Tickets.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new Ticket());

            await _sut.DeleteAsync(It.IsAny<Guid>(), ticketId);

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.Tickets.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsUnlinkTicketsMethod_WhenTicketExists()
        {
            var ticketId = Guid.NewGuid();

            var ticket = new Ticket
            {
                LinkedTicketIds = new List<Guid> { ticketId }
            };

            _unitOfWorkMock
               .Setup(x => x.Tickets.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
               .ReturnsAsync(ticket);

            await _sut.DeleteAsync(It.IsAny<Guid>(), Guid.NewGuid());

            _ticketLinkServiceMock.Verify(
                ticketLinkService => ticketLinkService.UnlinkTicketsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_UpdatesStatus_WhenTicketExists()
        {
            var model = new Ticket();

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tickets.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(model);

            await _sut.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Status>());

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.Tickets.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Ticket>()), Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_ThrowsException_WhenTicketNotExists()
        {
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tickets.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.UpdateStatusAsync(It.IsAny<Guid>(), Guid.NewGuid(), It.IsAny<Status>()));
        }

        [Fact]
        public async Task GetCount_ShouldReturnCorrectNumberOfTickets_IfModelIsValid()
        {
            var stub = new FilterDto
            {
                SelectedStatuses = new List<Status>(),
                SelectedPriorities = new List<Priority>()
            };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tickets.GetCountAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Ticket, bool>>>()))
                .ReturnsAsync(1);

            var result = await _sut.GetCountAsync(It.IsAny<Guid>(), stub);

            Assert.Equal(1, result);
        }
    }
}