using System;
using System.Threading.Tasks;
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
    public class UserManagesTicketTests : TestBase
    {
        private readonly TicketsController _sut;

        private TicketApiModel _inputCreateTicket;
        private Ticket _createdTicket;

        private Ticket _existingUpdatedTicket;
        private TicketApiModel _inputUpdatedTicket;
        private Ticket _outputUpdatedTicket;

        private Ticket _existingTicketToDelete;

        public UserManagesTicketTests()
        {
            MapperSetUp();
            var communicationServiceMock = new Mock<ICommunicationService>();

            var ticketsControllerLogMock = GetLoggerMock<TicketsController>();
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
                ticketsControllerLogMock.Object);
            SetupRequestHeader(_sut);
        }

        [Fact]
        public void UserCreatesTicket()
        {
            var ticketToCreate = new TicketApiModel
            {
                Name = "Test_Name",
                Text = "Test_Text",
                Priority = Priority.Medium,
                Status = Status.InProgress
            };

            this.Given(s => s.GivenATicketThatUserWantsToCreate(ticketToCreate))
                .When(s => s.WhenUserCreatesTicket())
                .Then(s => s.TheTicketWithTheSameNameWasCreated())
                .And(s => s.AndWithTheSameText())
                .And(s => s.AndWithTheSamePriority())
                .And(s => s.AndWithTheSameStatus())
                .BDDfy<UserManagesTickets>();
        }

        [Fact]
        public void UserUpdatesTicket()
        {
            var ticket = new Ticket
            {
                Name = "Test_Name",
                Text = "Test_Text",
                Priority = Priority.Medium,
                Status = Status.InProgress
            };
            var updatedTicket = new TicketApiModel
            {
                Name = "Updated_Test",
                Text = "Updated_Test_Text",
                Priority = Priority.High,
                Status = Status.InProgress
            };

            this.Given(s => s.GivenAnExistingTicketThatUserWantsToUpdate(ticket))
                .And(s => s.AndAnUpdatedVersionOfExistingTicketThatAUserWantsToUpdate(updatedTicket))
                .When(s => s.WhenUserUpdatesTicket())
                .Then(s => s.ThenTicketHasBeenUpdatedWithTheRightId())
                .And(s => s.AndWithTheRightName())
                .And(s => s.AndWithTheRightText())
                .And(s => s.AndWithTheRightPriority())
                .And(s => s.AndWithTheRightStatus())
                .BDDfy<UserManagesTickets>();
        }

        [Fact]
        public void UserDeletesTickets()
        {
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Name = "Test_Name",
                Text = "Test_Text",
                Priority = Priority.Medium,
                Status = Status.InProgress
            };

            this.Given(s => s.GivenExistingTicketThatUserWantsToDelete(ticket))
                .When(s => s.WhenUserDeletesThisTicket())
                .Then(s => s.ThenTicketHasBeenDeleted())
                .BDDfy<UserManagesTickets>();
        }

        public override void Dispose()
        {
            if(_inputCreateTicket != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _inputCreateTicket.Id)).Wait();
            }

            if(_existingTicketToDelete != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _existingTicketToDelete.Id)).Wait();
            }

            if(_existingUpdatedTicket != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _existingUpdatedTicket.Id)).Wait();
            }
        }

        private void GivenATicketThatUserWantsToCreate(TicketApiModel apiModel)
        {
            _inputCreateTicket = apiModel;
        }

        private async Task GivenAnExistingTicketThatUserWantsToUpdate(Ticket ticket)
        {
            ticket.Id = await UnitOfWork.Tickets.CreateAsync(StubTeamId, ticket);
            _existingUpdatedTicket = ticket;
        }

        private void AndAnUpdatedVersionOfExistingTicketThatAUserWantsToUpdate(TicketApiModel apiModel)
        {
            apiModel.Id = _existingUpdatedTicket.Id;
            _inputUpdatedTicket = apiModel;
        }

        private async Task GivenExistingTicketThatUserWantsToDelete(Ticket ticket)
        {
            ticket.Id = await UnitOfWork.Tickets.CreateAsync(StubTeamId, ticket);
            _existingTicketToDelete = ticket;
        }

        private async Task WhenUserCreatesTicket()
        {
            var createdTicketId = await _sut.Create(StubTeamId, _inputCreateTicket);
            var okObjectResult = createdTicketId as OkObjectResult;
            if (okObjectResult != null)
            {
                _inputCreateTicket.Id = (Guid)okObjectResult.Value;
            }
        }

        private async Task WhenUserUpdatesTicket()
        {
            await _sut.Update(StubTeamId, _inputUpdatedTicket);

            _outputUpdatedTicket = await UnitOfWork.Tickets.GetAsync(StubTeamId, _inputUpdatedTicket.Id);
        }

        private async Task WhenUserDeletesThisTicket()
        {
            await _sut.Delete(StubTeamId, _existingTicketToDelete.Id);
        }

        private async Task TheTicketWithTheSameNameWasCreated()
        {
            _createdTicket = await UnitOfWork.Tickets.GetAsync(StubTeamId, _inputCreateTicket.Id);
            Assert.Equal(_inputCreateTicket.Name, _createdTicket.Name);
        }

        private void AndWithTheSameText()
        {
            Assert.Equal(_inputCreateTicket.Text, _createdTicket.Text);
        }

        private void AndWithTheSamePriority()
        {
            Assert.Equal(_inputCreateTicket.Priority, _createdTicket.Priority);
        }

        private void AndWithTheSameStatus()
        {
            Assert.Equal(_inputCreateTicket.Status, _createdTicket.Status);
        }

        private void ThenTicketHasBeenUpdatedWithTheRightId()
        {
            Assert.Equal(_inputUpdatedTicket.Id, _outputUpdatedTicket.Id);
        }

        private void AndWithTheRightName()
        {
            Assert.Equal(_inputUpdatedTicket.Name, _outputUpdatedTicket.Name);
        }

        private void AndWithTheRightText()
        {
            Assert.Equal(_inputUpdatedTicket.Text, _outputUpdatedTicket.Text);
        }

        private void AndWithTheRightPriority()
        {
            Assert.Equal(_inputUpdatedTicket.Priority, _outputUpdatedTicket.Priority);
        }

        private void AndWithTheRightStatus()
        {
            Assert.Equal(_inputUpdatedTicket.Status, _outputUpdatedTicket.Status);
        }

        private async Task ThenTicketHasBeenDeleted()
        {
            Assert.Null(await UnitOfWork.Tickets.GetAsync(StubTeamId, _existingTicketToDelete.Id));
        }
    }
}