using System;
using System.Collections.Generic;
using Moq;
using StatisticService.BLL.DTO;
using StatisticService.BLL.Interfaces;
using Xunit;
using StatisticService.Tests.Core.Attributes;
using StatisticService.Tests.Core.Enums;

namespace StatisticService.BLL.Tests.Services
{
    [Category(TestType.Unit)]
    public class StatisticServiceTest
    {
        private readonly IStatisticService _sut;

        public StatisticServiceTest()
        {
            _sut = new BLL.Services.StatisticService();
        }

        [Fact]
        public void GetStatisticFiltered_ReturnsStatisticForLastTwoWeeks_WhenDateFromIsDefault()
        {
            const int defaultStatisticDaysNumber = 14;
            var ticket = new TicketDto();
            var ticketDtos = new List<TicketDto> { ticket };

            var result = _sut.GetStatisticFiltered(It.IsAny<DateTime>(), ticketDtos);

            Assert.Equal(defaultStatisticDaysNumber, result.DateCountOfTicketsDictionary.Count);
        }

        [Fact]
        public void GetStatisticFiltered_ReturnsStatisticFromStartDate()
        {
            const int statisticForDaysNumber = 5;
            var listTickets = new List<TicketDto> { new TicketDto() };

            var dateFrom = DateTime.UtcNow.AddDays(-statisticForDaysNumber);

            var result = _sut.GetStatisticFiltered(dateFrom, listTickets);

            Assert.Equal(statisticForDaysNumber, result.DateCountOfTicketsDictionary.Count);
        }

        [Fact]
        public void GetStatisticFiltered_ReturnsNull_WhenCollectionIsEmpty()
        {
            var ticketDtos = new List<TicketDto>();

            var result = _sut.GetStatisticFiltered(It.IsAny<DateTime>(), ticketDtos);

            Assert.Null(result);
        }
    }
}