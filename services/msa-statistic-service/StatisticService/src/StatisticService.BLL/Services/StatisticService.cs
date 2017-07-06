using System;
using System.Collections.Generic;
using System.Linq;
using StatisticService.BLL.DTO;
using StatisticService.BLL.Interfaces;
using StatisticService.Core.Enums;

namespace StatisticService.BLL.Services
{
    public class StatisticService : IStatisticService
    {
        private const int DefaultStatisticDaysNumber = 14;

        public UserStatisticDto GetStatisticFiltered(DateTime startDate, List<TicketDto> tickets)
        {
            if (!tickets.Any())
            {
                return null;
            }

            var statisticDaysNumber = GetStatisticDaysNumber(startDate);
            startDate = GetStartDate(startDate, statisticDaysNumber);

            var result = new UserStatisticDto
            {
                Tickets = tickets,
                DateCountOfTicketsDictionary = CountTicketsPerEachDay(tickets, startDate, statisticDaysNumber),
                StatusCountDictionary = GetStatusCountDictionary(tickets),
                PriorityCountDictionary = GetPriorityCountDictionary(tickets)
            };

            return result;
        }

        private DateTime GetStartDate(DateTime startDate, int statisticDaysNumber)
        {
            return startDate == default(DateTime) ? DateTime.UtcNow.AddDays(-statisticDaysNumber) : startDate;
        }

        private int GetStatisticDaysNumber(DateTime startDate)
        {
            return startDate == default(DateTime) ? DefaultStatisticDaysNumber : (DateTime.UtcNow - startDate).Days;
        }

        private Dictionary<Priority, int> GetPriorityCountDictionary(IList<TicketDto> tickets)
        {
            var priorityCountDictionary = new Dictionary<Priority, int>();

            foreach (Priority priority in Enum.GetValues(typeof(Priority)))
            {
                var statusCount = tickets.Count(ticketDto => ticketDto.Priority == priority);
                priorityCountDictionary.Add(priority, statusCount);
            }

            return priorityCountDictionary;
        }

        private Dictionary<Status, int> GetStatusCountDictionary(IList<TicketDto> tickets)
        {
            var statusCountDictionary = new Dictionary<Status, int>();

            foreach (Status status in Enum.GetValues(typeof(Status)))
            {
                var statusCount = tickets.Count(ticketDto => ticketDto.Status == status);
                statusCountDictionary.Add(status, statusCount);
            }

            return statusCountDictionary;
        }

        private Dictionary<DateTime, int> CountTicketsPerEachDay(
            IList<TicketDto> tickets,
            DateTime startDate,
            int daysInterval)
        {
            var ticketsPerEachDay = new Dictionary<DateTime, int>();

            for (var i = 1; i <= daysInterval; i++)
            {
                var date = startDate.AddDays(i);
                var ticketCount = tickets.Count(ticketDto => ticketDto.CreationDate.Date == date.Date);

                ticketsPerEachDay.Add(date, ticketCount);
            }

            return ticketsPerEachDay;
        }
    }
}