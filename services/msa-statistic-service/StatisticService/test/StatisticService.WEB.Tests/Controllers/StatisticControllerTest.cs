using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StatisticService.BLL.DTO;
using StatisticService.BLL.Interfaces;
using StatisticService.WEB.Controllers;
using StatisticService.WEB.Models.StatisticApiModels;
using Xunit;
using StatisticService.Tests.Core.Attributes;
using StatisticService.Tests.Core.Enums;
using EpamMA.Communication.Interfaces;

namespace StatisticService.WEB.Tests.Controllers
{
    [Category(TestType.Unit)]
    public class StatisticControllerTest : TestBase
    {
        private readonly Mock<IStatisticService> _statisticServiceMock;
        private readonly Mock<ICommunicationService> _communicationServiceMock;
        private readonly StatisticController _sut;

        public StatisticControllerTest()
        {
            SetUp();
            _statisticServiceMock = new Mock<IStatisticService>();
            _communicationServiceMock = new Mock<ICommunicationService>();
            var mockLog = new Mock<ILogger<StatisticController>>();
            _sut = new StatisticController(
                _statisticServiceMock.Object, 
                Mapper, 
                _communicationServiceMock.Object,
                mockLog.Object);
        }

        [Fact]
        public async Task GetStatistic_CallsGetStatisticFilteredMethod_WhenInputIsValid()
        {
            var date = DateTime.MaxValue;
         
            SetupRequestHeader(_sut);

            await _sut.GetStatistic(Guid.NewGuid(), date);

            _statisticServiceMock.Verify(
                statistic => statistic.GetStatisticFiltered(
                    It.IsAny<DateTime>(), 
                    It.IsAny<List<TicketDto>>()), 
                Times.Once);
        }

        [Fact]
        public async Task GetStatistic_ReturnsJsonWithData_WhenInputIsValid()
        {
            var date = DateTime.MaxValue;
          
            SetupRequestHeader(_sut);

            var result = await _sut.GetStatistic(Guid.NewGuid(), date) as JsonResult;

            Assert.NotNull(result?.Value);
        }

        [Fact]
        public async Task GetStatistic_CallsGetAsyncMethod_WhenInputIsValid()
        {
            var date = DateTime.MaxValue;

            SetupRequestHeader(_sut);

            _communicationServiceMock
                .Setup(communication => communication.GetAsync<IEnumerable<TicketApiModel>>(It.IsAny<string>(), null, null, It.IsAny<string>()))
                .ReturnsAsync(new List<TicketApiModel>());

            await _sut.GetStatistic(Guid.NewGuid(), date);

            _communicationServiceMock.Verify(
                communication => communication.GetAsync<IEnumerable<TicketApiModel>>(It.IsAny<string>(), null, null, It.IsAny<string>()),
                Times.Once);
        }
    }
}