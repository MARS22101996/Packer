using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    public class UserManagesWithTicketAssignees : TestBase
    {
        private readonly TicketsController _sut;
        private readonly Mock<ICommunicationService> _communicationServiceMock;

        private Ticket _existingUnassignedTicket;
        private UserApiModel _stubUser;

        private Ticket _existingAssignedTicket;

        public UserManagesWithTicketAssignees()
        {
            MapperSetUp();
            _communicationServiceMock = new Mock<ICommunicationService>();

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
                _communicationServiceMock.Object,
                ticketsControllerLogMock.Object);
            SetupRequestHeader(_sut);
        }

        public override void Dispose()
        {
            if(_existingUnassignedTicket != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _existingUnassignedTicket.Id)).Wait();
            }

            if(_existingAssignedTicket != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _existingAssignedTicket.Id)).Wait();
            }
        }

        [Fact]
        public void UserAssignsUserToTicket()
        {
            var testTicket = new Ticket
            {
                Name = "Test_Name",
                Text = "Test_Text",
                Priority = Priority.Medium,
                Status = Status.InProgress
            };

            var testUser = new UserApiModel
            {
                Id = Guid.NewGuid(),
                Email = "Test_Email"
            };

            _communicationServiceMock.Setup(x => x.GetAsync<UserApiModel>(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<IHeaderDictionary>(),
                It.IsAny<string>())).ReturnsAsync(testUser);

            this.Given(s => s.GivenAnExistingTicketThatUserWantsToAssign(testTicket))
                .And(s => s.UserThatHasToBeAssigned(testUser))
                .When(s => s.WhenUserAssignsUserToTicket())
                .Then(s => s.ThenUserIsAssignedToThisTicket())
                .BDDfy<UserManagesAssignees>();
        }

        [Fact]
        public void UserUnassignesUserFromTicket()
        {
            var testTicket = new Ticket
            {
                Name = "Test_Name",
                Text = "Test_Text",
                Priority = Priority.Medium,
                Status = Status.InProgress
            };

            this.Given(s => s.GivenExistingTicketUserWantsToUnassign(testTicket))
                .When(s => s.WhenUserUnassignsThisTicket())
                .Then(s => s.ThenTicketIsUnassigned())
                .BDDfy<UserManagesAssignees>();
        }

        private async Task GivenAnExistingTicketThatUserWantsToAssign(Ticket apiModel)
        {
            apiModel.Id = await UnitOfWork.Tickets.CreateAsync(StubTeamId, apiModel);
            _existingUnassignedTicket = apiModel;
        }

        private void UserThatHasToBeAssigned(UserApiModel userApiModel)
        {
            _stubUser = userApiModel;
        }

        private async Task GivenExistingTicketUserWantsToUnassign(Ticket ticket)
        {
            ticket.Id = await UnitOfWork.Tickets.CreateAsync(StubTeamId, ticket);
            _existingAssignedTicket = ticket;
        }

        private async Task WhenUserAssignsUserToTicket()
        {
            await _sut.Assign(StubTeamId, _existingUnassignedTicket.Id, _stubUser.Id);
        }

        private async Task WhenUserUnassignsThisTicket()
        {
            await _sut.Unassign(StubTeamId, _existingAssignedTicket.Id);
        }

        private async Task ThenUserIsAssignedToThisTicket()
        {
            var assignedOutputTicket = await UnitOfWork.Tickets.GetAsync(StubTeamId, _existingUnassignedTicket.Id);
            Assert.Equal(assignedOutputTicket.Assignee.Id, _stubUser.Id);
        }

        private async Task ThenTicketIsUnassigned()
        {
            var updatedTicketWithNoAssignee = await UnitOfWork.Tickets.GetAsync(StubTeamId, _existingAssignedTicket.Id);
            Assert.Contains(Guid.Empty.ToString(), updatedTicketWithNoAssignee.Assignee.Id.ToString());
        }
    }
}