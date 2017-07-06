using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using TicketService.BLL.DTO;
using TicketService.BLL.Interfaces;
using TicketService.BLL.Services;
using TicketService.DAL.Entities;
using TicketService.DAL.Interfaces;
using TicketService.Tests.Core.Attributes;
using TicketService.Tests.Core.Enums;
using Xunit;

namespace TicketService.BLL.Tests.Services
{
    [Category(TestType.Unit)]
    public class CommentServiceTest : TestBase
    {
        private readonly ICommentService _commentService;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public CommentServiceTest()
        {
            SetUp();
            var mockLog = new Mock<ILogger<CommentService>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _commentService = new CommentService(_unitOfWorkMock.Object, Mapper, mockLog.Object);
        }

        [Fact]
        public async Task GetAll_CallsFindMethod_IfInputIsValid()
        {
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Comments.GetAllAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<Comment>());

            await _commentService.GetAllAsync(It.IsAny<Guid>(), It.IsAny<Guid>());

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.Comments.GetAllAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnsNotEmptyListOfComments_IfDataExists()
        {
            var comments = new List<Comment>
            {
                new Comment { User = new User() },
                new Comment { User = new User() }
            };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Comments.GetAllAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(comments);

            var commentList = await _commentService.GetAllAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.True(commentList.Count() != 0);
        }

        [Fact]
        public async Task CreateAsync_ThrowsException_WhenInputIsNull()
        {
            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Comments.CreateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Comment>()));

            await Assert.ThrowsAsync<NullReferenceException>(() => _commentService.CreateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), null));
        }

        [Fact]
        public async Task CreateAsync_CallsCreateAsyncMethod_WhenValidComment()
        {
            var model = new CommentDto();
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Comments.CreateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Comment>()))
                .ReturnsAsync(Guid.NewGuid());

            await _commentService.CreateAsync(Guid.NewGuid(), Guid.NewGuid(), model);

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.Comments.CreateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Comment>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsDeleteAsyncMethod_WhenValidId()
        {
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Comments.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            await _commentService.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>());

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.Comments.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }
    }
}