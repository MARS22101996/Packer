using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TicketService.BLL.DTO;
using TicketService.BLL.Interfaces;
using TicketService.Tests.Core.Attributes;
using TicketService.Tests.Core.Enums;
using TicketService.WEB.Controllers;
using Xunit;

namespace TicketService.WEB.Tests.Controllers
{
    [Category(TestType.Unit)]
    public class TagControllerTest : TestBase
    {
        private readonly Mock<ITagService> _tagServiceMock;
        private readonly TagsController _sut;

        public TagControllerTest()
        {
            SetUp();

            _tagServiceMock = new Mock<ITagService>();
            var loggerMock = new Mock<ILogger<TagsController>>();

            _sut = new TagsController(
                _tagServiceMock.Object,
                Mapper,
                loggerMock.Object);
        }

        [Fact]
        public void GetTags_ReturnsTagsInJson_WhenTagsExists()
        {
            _tagServiceMock.Setup(method => method.GetAll()).Returns(new List<TagDto>());

            var result = _sut.GetTags();
            var jsonResult = result as JsonResult;

            Assert.NotNull(jsonResult?.Value);
        }
    }
}
