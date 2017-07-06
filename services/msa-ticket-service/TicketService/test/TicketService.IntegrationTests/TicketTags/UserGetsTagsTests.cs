using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TestStack.BDDfy;
using TicketService.BLL.Services;
using TicketService.Tests.Core.Attributes;
using TicketService.Tests.Core.Enums;
using TicketService.WEB.Controllers;
using TicketService.WEB.Models;
using Xunit;

namespace TicketService.IntegrationTests.TicketTags
{
    [Category(TestType.Integration)]
    public class UserGetsTagsTests : TestBase
    {
        private readonly TagsController _sut;

        private int _tagsCount;
        private IEnumerable<TagApiModel> _tagsOutput;

        public UserGetsTagsTests()
        {
            MapperSetUp();

            var tagsControllerLogMock = GetLoggerMock<TagsController>();
            var tagServiceLogMock = GetLoggerMock<TagService>();

            var tagService = new TagService(UnitOfWork, Mapper, tagServiceLogMock.Object);
            _sut = new TagsController(
                tagService,
                Mapper,
                tagsControllerLogMock.Object);
        }

        [Fact]
        public void GetTags()
        {
            this.Given(s => s.GivenExistingAmountOfTags())
                .When(s => s.WhenUserNeedsHelpWhileCreatingTickets())
                .Then(s => s.ThenUserReceivesASerializableTicketList())
                .And(s => s.AndUserReceivesAllAvailableTagsForHints())
                .BDDfy<UserGetsHelpTags>();
        }

        private void GivenExistingAmountOfTags()
        {
            _tagsCount = UnitOfWork.Tags.GetAll().Count();
        }

        private void WhenUserNeedsHelpWhileCreatingTickets()
        {
            var jsonResult = _sut.GetTags() as JsonResult;
            if (jsonResult != null)
            {
                _tagsOutput = jsonResult.Value as IEnumerable<TagApiModel>;
            }
        }

        private void ThenUserReceivesASerializableTicketList()
        {
            Assert.NotNull(_tagsOutput);
        }

        private void AndUserReceivesAllAvailableTagsForHints()
        {
            Assert.Equal(_tagsCount, _tagsOutput.Count());
        }
    }
}