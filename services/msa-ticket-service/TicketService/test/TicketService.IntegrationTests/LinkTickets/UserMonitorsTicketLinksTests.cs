using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestStack.BDDfy;
using TicketService.BLL.DTO;
using TicketService.BLL.Services;
using TicketService.DAL.Entities;
using TicketService.Tests.Core.Attributes;
using TicketService.Tests.Core.Enums;
using TicketService.WEB.Controllers;
using TicketService.WEB.Models;
using Xunit;

namespace TicketService.IntegrationTests.LinkTickets
{
    [Category(TestType.Integration)]
    public class UserMonitorsTicketLinksTests : TestBase
    {
        private readonly LinkedTicketsController _sut;

        private Ticket _firstExistingLinkedTicket;
        private Ticket _secondExistingLinkedTicket;
        private List<TicketApiModel> _firstTicketUpdatedLinkedTickets;

        private Ticket _firstExistingUnlinkedTicket;
        private Ticket _secondExistingUnlinkedTicket;
        private List<TicketApiModel> _firstTicketUpdatedUnlinkedTickets;

        public UserMonitorsTicketLinksTests()
        {
            MapperSetUp();
            var ticketsControllerLogMock = GetLoggerMock<LinkedTicketsController>();
            var ticketLinkServiceLogMock = GetLoggerMock<TicketLinkService>();

            var ticketLinkService = new TicketLinkService(UnitOfWork, Mapper, ticketLinkServiceLogMock.Object);

            _sut = new LinkedTicketsController(
                ticketLinkService,
                ticketsControllerLogMock.Object,
                Mapper);
            SetupRequestHeader(_sut);
        }

        [Fact]
        public void UserGetsLinkedTickets()
        {
            var firstTestTicket = new Ticket
            {
                Text = "Test_Text",
                Name = "Test_Name"
            };
            var secondTestTicket = new Ticket
            {
                Text = "Test_Text",
                Name = "Test_Name"
            };

            this.Given(s => s.GivenTicketsThatAreLinked(firstTestTicket, secondTestTicket))
                .When(s => s.WhenUserGetsLinkedTicketsForTheFirstTicket())
                .Then(s => s.ThenUserReceivesASerializableTicketList())
                .And(s => s.AndUserReceivesOnlyLinkedTickets())
                .BDDfy<UserMonitorsTicketLinks>();
        }

        [Fact]
        public void UserGetUnlinkedTickets()
        {
            var firstTestTicket = new Ticket
            {
                Text = "Test_Text",
                Name = "Test_Name"
            };
            var secondTestTicket = new Ticket
            {
                Text = "Test_Text",
                Name = "Test_Name"
            };

            this.Given(s => s.GivenTwoTicketsThatAreUnlinked(firstTestTicket, secondTestTicket))
                .When(s => s.WhenUserRequestsUnlinkedTicketsForTheFirstTicket())
                .Then(s => s.ThenUserReceivesASerializableUnlinkedTicketList())
                .And(s => s.AndUserReceivesOnlyUnlinkedTickets())
                .BDDfy<UserMonitorsTicketLinks>();
        }

        public override void Dispose()
        {
            if(_firstExistingLinkedTicket != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _firstExistingLinkedTicket.Id)).Wait();
            }

            if(_secondExistingLinkedTicket != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _secondExistingLinkedTicket.Id)).Wait();
            }

            if (_firstExistingUnlinkedTicket != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _firstExistingUnlinkedTicket.Id)).Wait();
            }

            if (_secondExistingUnlinkedTicket != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _secondExistingUnlinkedTicket.Id)).Wait();
            }
        }

        private async Task GivenTicketsThatAreLinked(Ticket firstTicket, Ticket secondTicket)
        {
            var firstTicketId = await UnitOfWork.Tickets.CreateAsync(StubTeamId, firstTicket);
            var secondTicketId = await UnitOfWork.Tickets.CreateAsync(StubTeamId, secondTicket);

            var firstCreatedTicket = await UnitOfWork.Tickets.GetAsync(StubTeamId, firstTicketId);
            var secondCreatedTicket = await UnitOfWork.Tickets.GetAsync(StubTeamId, secondTicketId);

            firstCreatedTicket.LinkedTicketIds = firstCreatedTicket.LinkedTicketIds.Append(secondTicketId);
            secondCreatedTicket.LinkedTicketIds = secondCreatedTicket.LinkedTicketIds.Append(firstTicketId);

            await UnitOfWork.Tickets.UpdateAsync(StubTeamId, firstCreatedTicket);
            await UnitOfWork.Tickets.UpdateAsync(StubTeamId, secondCreatedTicket);

            _firstExistingLinkedTicket = firstCreatedTicket;
            _secondExistingLinkedTicket = secondCreatedTicket;
        }

        private async Task GivenTwoTicketsThatAreUnlinked(Ticket firstTicket, Ticket secondTicket)
        {
            var firstTicketId = await UnitOfWork.Tickets.CreateAsync(StubTeamId, firstTicket);
            var secondTicketId = await UnitOfWork.Tickets.CreateAsync(StubTeamId, secondTicket);

            var firstCreatedTicket = await UnitOfWork.Tickets.GetAsync(StubTeamId, firstTicketId);
            var secondCreatedTicket = await UnitOfWork.Tickets.GetAsync(StubTeamId, secondTicketId);

            _firstExistingUnlinkedTicket = firstCreatedTicket;
            _secondExistingUnlinkedTicket = secondCreatedTicket;
        }

        private async Task WhenUserGetsLinkedTicketsForTheFirstTicket()
        {
            var jsonResult =await _sut.GetLinkedTickets(StubTeamId, _secondExistingLinkedTicket.Id) as JsonResult;
            if (jsonResult != null)
            {
                _firstTicketUpdatedLinkedTickets = jsonResult.Value as List<TicketApiModel>;
            }
        }

        private async Task WhenUserRequestsUnlinkedTicketsForTheFirstTicket()
        {
            var jsonResult = await _sut.GetUnLinkedTickets(StubTeamId, _secondExistingUnlinkedTicket.Id) as JsonResult;
            if (jsonResult != null)
            {
                _firstTicketUpdatedUnlinkedTickets = jsonResult.Value as List<TicketApiModel>;
            }
        }

        private void ThenUserReceivesASerializableTicketList()
        {
            Assert.NotNull(_firstTicketUpdatedLinkedTickets);
        }

        private void AndUserReceivesOnlyLinkedTickets()
        {
            Assert.Contains(_secondExistingLinkedTicket.Id, _firstTicketUpdatedLinkedTickets.First().LinkedTicketIds);
        }

        private void ThenUserReceivesASerializableUnlinkedTicketList()
        {
            Assert.NotNull(_firstTicketUpdatedUnlinkedTickets);
        }

        private void AndUserReceivesOnlyUnlinkedTickets()
        {
            var ticketDto = Mapper.Map<TicketDto>(_firstExistingUnlinkedTicket);
            var ticketApiModel = Mapper.Map<TicketApiModel>(ticketDto);
            Assert.Contains(ticketApiModel.Id, _firstTicketUpdatedUnlinkedTickets.Select(x => x.Id));
        }
    }
}