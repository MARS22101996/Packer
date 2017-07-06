using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestStack.BDDfy;
using TicketService.BLL.Services;
using TicketService.DAL.Entities;
using TicketService.Tests.Core.Attributes;
using TicketService.Tests.Core.Enums;
using TicketService.WEB.Controllers;
using TicketService.WEB.Models;
using Xunit;
using EpamMA.Communication.Interfaces;

namespace TicketService.IntegrationTests.TicketComments
{
    [Category(TestType.Integration)]
    public class UserGetsCommentsTests : TestBase
    {
        private readonly CommentsController _sut;

        private Ticket _existingTicket;
        private IEnumerable<CommentApiModel> _outputComment;

        public UserGetsCommentsTests()
        {
            MapperSetUp();
            var communicationServiceMock = new Mock<ICommunicationService>();

            var commentsServiceLogMock = GetLoggerMock<CommentService>();
            var commentsControllerLogMock = GetLoggerMock<CommentsController>();

            var commentsService = new CommentService(UnitOfWork, Mapper, commentsServiceLogMock.Object);
            _sut = new CommentsController(
                Mapper,
                commentsService,
                commentsControllerLogMock.Object,
                communicationServiceMock.Object);
        }

        public override void Dispose()
        {
            if (_existingTicket != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _existingTicket.Id)).Wait();
            }
        }

        [Fact]
        public void UserGetsCommentForTicket()
        {
            var testTicket = new Ticket
            {
                Name = "Test_Name",
                Text = "Test_Text"
            };

            var testComment = new Comment
            {
                Text = "Test_Text",
                User = new User()
            };

            this.Given(s => s.GivenExistingTicket(testTicket))
                .And(s => s.AndGivenExistingComment(testComment))
                .When(s => s.WhenUserGetsComment())
                .Then(s => s.ThenUserReceivesASerializableTicket())
                .And(s => s.AndUserReceivedCommentWithTheSameId())
                .And(s => s.AndWithTheSameDate())
                .And(s => s.AndWithTheSameUser())
                .BDDfy<UserReadsComments>();
        }

        private async Task GivenExistingTicket(Ticket ticket)
        {
            ticket.Id = await UnitOfWork.Tickets.CreateAsync(StubTeamId, ticket);
            _existingTicket = ticket;
        }

        private async Task AndGivenExistingComment(Comment comment)
        {
            comment.Id = await UnitOfWork.Comments.CreateAsync(StubTeamId, _existingTicket.Id, comment);
            _existingTicket.Comments = _existingTicket.Comments.Append(comment);
        }

        private async Task WhenUserGetsComment()
        {
            var jsonResult = await _sut.Get(StubTeamId, _existingTicket.Id) as JsonResult;
            if (jsonResult != null)
            {
                _outputComment = jsonResult.Value as IEnumerable<CommentApiModel>;
            }
        }

        private void ThenUserReceivesASerializableTicket()
        {
            Assert.NotNull(_outputComment);
        }

        private void AndUserReceivedCommentWithTheSameId()
        {
            Assert.Equal(_existingTicket.Comments.ElementAt(0).Id, _outputComment.ElementAt(0).Id);
        }

        private void AndWithTheSameDate()
        {
            Assert.Equal(_existingTicket.Comments.ElementAt(0).Date, _outputComment.ElementAt(0).Date);
        }

        private void AndWithTheSameUser()
        {
            Assert.Equal(_existingTicket.Comments.ElementAt(0).User.Id, _outputComment.ElementAt(0).User.Id);
        }
    }
}