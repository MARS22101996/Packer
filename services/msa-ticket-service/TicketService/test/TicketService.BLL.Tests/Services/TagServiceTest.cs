using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
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
    public class TagServiceTest : TestBase
    {
        private readonly ITagService _sut;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public TagServiceTest()
        {
            SetUp();
            var mockLog = new Mock<ILogger<TagService>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new TagService(_unitOfWorkMock.Object, Mapper, mockLog.Object);
        }

        [Fact]
        public void GetAll_ReturnsTags_IfDataExists()
        {
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tags.GetAll())
                .Returns(new List<Tag>().AsQueryable());

            var res = _sut.GetAll().ToList();

            Assert.NotNull(res);
        }

        [Fact]
        public void Add_AddTag_IfDataExists()
        {
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Tags.GetAll())
                .Returns(new List<Tag>().AsQueryable());

            _sut.AddAsync(new List<string> { "tag-stub" });

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.Tags.CreateAsync(It.IsAny<Tag>()), Times.Once);
        }
    }
}