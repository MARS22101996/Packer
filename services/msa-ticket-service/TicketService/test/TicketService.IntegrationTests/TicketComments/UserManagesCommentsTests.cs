using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    public class UserManagesCommentsTests : TestBase
    {
        private readonly CommentsController _sut;
        private readonly Mock<ICommunicationService> _communicationServiceMock;

        private Ticket _stubTicketForAddingComment;

        private CommentApiModel _addedComment;
        private List<Comment> _returnedComments;

        private Ticket _existingTicketWithCommentToDelete;

        public UserManagesCommentsTests()
        {
            MapperSetUp();

            _communicationServiceMock = new Mock<ICommunicationService>();
            var commentServiceLogMock = GetLoggerMock<CommentService>();
            var commentControllerLogMock = GetLoggerMock<CommentsController>();

            var commentService = new CommentService(UnitOfWork, Mapper, commentServiceLogMock.Object);
            _sut = new CommentsController(
                Mapper, 
                commentService, 
                commentControllerLogMock.Object, 
                _communicationServiceMock.Object);

            SetupRequestHeader(_sut);
        }

        public override void Dispose()
        {
            if(_stubTicketForAddingComment != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _stubTicketForAddingComment.Id)).Wait();
            }

            if (_existingTicketWithCommentToDelete != null)
            {
                Task.Run(() => UnitOfWork.Tickets.DeleteAsync(StubTeamId, _existingTicketWithCommentToDelete.Id)).Wait();
            }
        }

        [Fact]
        public void UserAddsComment()
        {
            var userId = Guid.NewGuid();
            var testTicket = new Ticket
            {
                Text = "Test_Text",
                Name = "Test_Name"
            };

            var testComment = new CommentApiModel
            {
                Text = "Test_text",
                Date = DateTime.UtcNow,
                User = new UserApiModel()
            };

            _communicationServiceMock.Setup(
                x => x.GetAsync<UserApiModel>(It.IsAny<string>(), null, It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
                .ReturnsAsync(new UserApiModel { Id = userId });

            this.Given(s => s.GivenExistingTicket(testTicket))
                .When(s => s.WhenUserAddsComment(testComment))
                .Then(s => s.ThenCommentIsAddedWithTheSameId())
                .And(s => s.AndWithTheSameText())
                .And(s => s.AndWithTheSameUserId())
                .BDDfy<UserManagesComments>();
        }

        [Fact]
        public void UserDeletesComment()
        {
            var testTicket = new Ticket
            {
                Text = "Test_Text",
                Name = "Test_Name"
            };

            var testComment = new Comment
            {
                Text = "Test_text",
                Date = DateTime.UtcNow,
                User = new User()
            };

            this.Given(s => s.GivenTicket(testTicket))
                .And(s => s.AndCommentThatUserWantsToDelete(testComment))
                .When(s => s.WhenUserDeletesComment())
                .Then(s => s.ThenCommentIsDeleted())
                .BDDfy<UserManagesComments>();
        }

        private async Task GivenExistingTicket(Ticket ticket)
        {
            ticket.Id = await UnitOfWork.Tickets.CreateAsync(StubTeamId, ticket);
            _stubTicketForAddingComment = ticket;
        }

        private async Task GivenTicket(Ticket ticket)
        {
            ticket.Id = await UnitOfWork.Tickets.CreateAsync(StubTeamId, ticket);
            _existingTicketWithCommentToDelete = ticket;
        }

        private async Task AndCommentThatUserWantsToDelete(Comment comment)
        {
            comment.Id = await UnitOfWork.Comments.CreateAsync(StubTeamId, _existingTicketWithCommentToDelete.Id, comment);
            _existingTicketWithCommentToDelete.Comments = _existingTicketWithCommentToDelete.Comments.Append(comment);
        }

        private async Task WhenUserAddsComment(CommentApiModel comment)
        {
            var okObjectResult = await _sut.Post(StubTeamId, _stubTicketForAddingComment.Id, comment) as OkObjectResult;
            if (okObjectResult != null)
            {
                comment.Id = (Guid) okObjectResult.Value;
            }
            _addedComment = comment;
        }

        private async Task WhenUserDeletesComment()
        {
            await _sut.Delete(StubTeamId,
                _existingTicketWithCommentToDelete.Id,
                _existingTicketWithCommentToDelete.Comments.ElementAt(0).Id);
        }

        private async Task ThenCommentIsAddedWithTheSameId()
        {
            _returnedComments = (await UnitOfWork.Comments.GetAllAsync(StubTeamId, _stubTicketForAddingComment.Id)).ToList();

            Assert.Equal(_returnedComments.ElementAt(0).Id, _addedComment.Id);
        }

        private void AndWithTheSameUserId()
        {
            Assert.Equal(_returnedComments.ElementAt(0).User.Id, _addedComment.User.Id);
        }

        private void AndWithTheSameText()
        {
            Assert.Equal(_returnedComments.ElementAt(0).Text, _addedComment.Text);
        }


        private async Task ThenCommentIsDeleted()
        {
            var databaseTicket = await UnitOfWork.Tickets.GetAsync(StubTeamId, _existingTicketWithCommentToDelete.Id);

            Assert.Equal(databaseTicket.Comments.Count(), 0);
        }
    }
}