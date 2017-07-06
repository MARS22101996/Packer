using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestStack.BDDfy;
using TicketService.BLL.Services;
using TicketService.Core.Enums;
using TicketService.DAL.Entities;
using TicketService.Tests.Core.Attributes;
using TicketService.Tests.Core.Enums;
using TicketService.WEB.Controllers;
using TicketService.WEB.Models;
using Xunit;
using EpamMA.Communication.Interfaces;

namespace TicketService.IntegrationTests.TicketsOps
{
    [Category(TestType.Integration)]
    public class UserRequestTicketsTests : TestBase
    {
        private readonly TicketsController _sut;

        private FilterApiModel _filterApiModel;
        private IEnumerable<TicketApiModel> _filteredTickets;
        private int _ticketCount;

        private Ticket _existingTicket;
        private TicketApiModel _returnTicket;

        public UserRequestTicketsTests()
        {
            MapperSetUp();
            var communicationServiceMock = new Mock<ICommunicationService>();

            var ticketControllerLogMock = GetLoggerMock<TicketsController>();
            var tagServiceLogMock = GetLoggerMock<TagService>();
            var ticketLinkServiceLogMock = GetLoggerMock<TicketLinkService>();
            var ticketServiceLogMock = GetLoggerMock<BLL.Services.TicketService>();

            var tagService = new TagService(UnitOfWork, Mapper, tagServiceLogMock.Object);
            var ticketLinkService = new TicketLinkService(UnitOfWork, Mapper, ticketLinkServiceLogMock.Object);
            var ticketService = new BLL.Services.TicketService(
                UnitOfWork, 
                Mapper, 
                tagService, 
                ticketLinkService, 
                ticketServiceLogMock.Object);

            _sut = new TicketsController(
                Mapper,
                ticketService,
                communicationServiceMock.Object,
                ticketControllerLogMock.Object);
        }

        [Fact]
        public void UserRequestsFilteredTicketsByPriority()
        {
            this.Given(s => s.GivenThePriorityFilter(Priority.Low))
                    .And(s => s.GivenFilteredTicketsAmountByPriority(Priority.Low))
                .When(s => s.WhenUserGetsFilteredTickets())
                .Then(s => s.ThenUserReceivesASerializableListOfTickets())
                .And(s => s.AndUserReceivesRightAmountOfTickets())
                .BDDfy<UserGetsTickets>();
        }

        [Fact]
        public void UserRequestSingleTicket()
        {
            var ticket = new Ticket
            {
                Name = "Test_Name",
                Text = "Test_Text",
                Priority = Priority.Medium,
                Status = Status.InProgress
            };

            this.Given(s => s.GivenAnExistingTicket(ticket))
                .When(s => s.WhenUserGetsThisTicketByTheSameId())
                .Then(s => s.ThenUserReceiveASerializableTicketApiModel(_returnTicket))
                .And(s => s.AndUserReceivesTheTicketWithTheSameId())
                .And(s => s.AndWithTheSameText())
                .And(s => s.AndWithTheSameName())
                .And(s => s.AndWithTheSamePriority())
                .And(s => s.AndWithTheSameStatus())
                .BDDfy<UserGetsTickets>();
        }

        public override void Dispose()
        {
            if(_existingTicket != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _existingTicket.Id)).Wait();
            }
        }

        private void GivenThePriorityFilter(Priority priority)
        {
            _filterApiModel = new FilterApiModel
            {
                SelectedPriorities = new List<Priority>
                {
                    priority
                }
            };
        }

        private async Task GivenAnExistingTicket(Ticket ticket)
        {
            ticket.Id = await UnitOfWork.Tickets.CreateAsync(StubTeamId, ticket);
            _existingTicket = ticket;
        }

        private async Task GivenFilteredTicketsAmountByPriority(Priority priority)
        {
            Expression<Func<Ticket, bool>> expression = p =>
                p.Priority.Equals(priority);

            _ticketCount = await UnitOfWork.Tickets.GetCountAsync(StubTeamId, expression.Expand());
        }

        private async Task WhenUserGetsFilteredTickets()
        {
            var jsonResult = await _sut.Get(StubTeamId, _filterApiModel) as JsonResult;
            if (jsonResult != null)
            {
                _filteredTickets = jsonResult.Value as IEnumerable<TicketApiModel>;
            }
        }

        private async Task WhenUserGetsThisTicketByTheSameId()
        {
            var jsonResult = await _sut.GetTicket(StubTeamId, _existingTicket.Id) as JsonResult;
            if (jsonResult != null)
            {
                _returnTicket = jsonResult.Value as TicketApiModel;
            }
        }

        private void ThenUserReceiveASerializableTicketApiModel(TicketApiModel model)
        {
            Assert.NotNull(model);
        }

        private void AndUserReceivesTheTicketWithTheSameId()
        {
            Assert.Equal(_returnTicket.Id, _existingTicket.Id);
        }

        private void AndWithTheSameName()
        {
            Assert.Equal(_returnTicket.Name, _existingTicket.Name);
        }

        private void AndWithTheSameText()
        {
            Assert.Equal(_returnTicket.Text, _existingTicket.Text);
        }

        private void AndWithTheSamePriority()
        {
            Assert.Equal(_returnTicket.Priority, _existingTicket.Priority);
        }

        private void AndWithTheSameStatus()
        {
            Assert.Equal(_returnTicket.Status, _existingTicket.Status);
        }

        private void ThenUserReceivesASerializableListOfTickets()
        {
            Assert.NotNull(_filteredTickets);
        }

        private void AndUserReceivesRightAmountOfTickets()
        {
            Assert.Equal(_filteredTickets.Count(), _ticketCount);
        }
    }
}