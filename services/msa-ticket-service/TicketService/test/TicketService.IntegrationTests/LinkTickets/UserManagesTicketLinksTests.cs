using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;
using TicketService.BLL.Services;
using TicketService.DAL.Entities;
using TicketService.Tests.Core.Attributes;
using TicketService.Tests.Core.Enums;
using TicketService.WEB.Controllers;
using Xunit;

namespace TicketService.IntegrationTests.LinkTickets
{
    [Category(TestType.Integration)]
    public class UserManagesTicketsLinksTests : TestBase
    {
        private readonly LinkedTicketsController _sut;

        private Ticket _firstExistingUnlinkedTicket;
        private Ticket _secondExistingUnlinkedTicket;

        private Ticket _firstExistingLinkedTicket;
        private Ticket _secondExistingLinkedTicket;

        public UserManagesTicketsLinksTests()
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
        public void LinkTickets()
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
                .When(s => s.WhenUserLinksTheseTickets())
                .Then(s => s.ThenTicketsAreLinked())
                .BDDfy<ManageTicketLink>();
        }

        [Fact]
        public void UnlinkTickets()
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

            this.Given(s => s.GivenTwoTicketsThatAreLinked(firstTestTicket, secondTestTicket))
                .When(s => s.WhenUserUnlinksTheseTickets())
                .Then(s => s.ThenTicketsAreUnlinked())
                .BDDfy<ManageTicketLink>();
        }

        public override void Dispose()
        { 
            if(_firstExistingUnlinkedTicket != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _firstExistingUnlinkedTicket.Id)).Wait();
            }

            if(_secondExistingUnlinkedTicket != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _secondExistingUnlinkedTicket.Id)).Wait();
            }

            if(_firstExistingLinkedTicket != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _firstExistingLinkedTicket.Id)).Wait();
            }

            if(_secondExistingLinkedTicket != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _secondExistingLinkedTicket.Id)).Wait();
            }
        }

        private async Task GivenTwoTicketsThatAreUnlinked(Ticket firstTicket, Ticket secondTicket)
        {
            var firstTicketId = await UnitOfWork.Tickets.CreateAsync(StubTeamId, firstTicket);
            var secondTicketId = await UnitOfWork.Tickets.CreateAsync(StubTeamId, secondTicket);

            _firstExistingUnlinkedTicket = await UnitOfWork.Tickets.GetAsync(StubTeamId, firstTicketId);
            _secondExistingUnlinkedTicket = await UnitOfWork.Tickets.GetAsync(StubTeamId, secondTicketId);
        }

        private async Task GivenTwoTicketsThatAreLinked(Ticket firstTicket, Ticket secondTicket)
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

        private async Task WhenUserLinksTheseTickets()
        {
            await _sut.LinkTickets(StubTeamId, _firstExistingUnlinkedTicket.Id, _secondExistingUnlinkedTicket.Id);
        }

        private async Task WhenUserUnlinksTheseTickets()
        {
            await _sut.UnLinkTickets(StubTeamId, _firstExistingLinkedTicket.Id, _secondExistingLinkedTicket.Id);
        }

        private async Task ThenTicketsAreLinked()
        {
            var firstUpdatedLinkedTicket = await UnitOfWork.Tickets.GetAsync(StubTeamId, _firstExistingUnlinkedTicket.Id);
            var secondUpdateLinkedTicket = await UnitOfWork.Tickets.GetAsync(StubTeamId, _secondExistingUnlinkedTicket.Id);

            Assert.Contains(_secondExistingUnlinkedTicket.Id, firstUpdatedLinkedTicket.LinkedTicketIds);
            Assert.Contains(_firstExistingUnlinkedTicket.Id, secondUpdateLinkedTicket.LinkedTicketIds);
        }

        private async Task ThenTicketsAreUnlinked()
        {
            var firstUpdatedLinkedTicket = await UnitOfWork.Tickets.GetAsync(StubTeamId, _firstExistingLinkedTicket.Id);
            var secondUpdateLinkedTicket = await UnitOfWork.Tickets.GetAsync(StubTeamId, _secondExistingLinkedTicket.Id);

            Assert.DoesNotContain(_firstExistingLinkedTicket.Id, firstUpdatedLinkedTicket.LinkedTicketIds);
            Assert.DoesNotContain(_secondExistingLinkedTicket.Id, secondUpdateLinkedTicket.LinkedTicketIds);
        }
    }
}