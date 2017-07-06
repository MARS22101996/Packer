using System;
using System.Collections.Generic;
using StatisticService.Core.Enums;

namespace StatisticService.BLL.DTO
{
    public class UserStatisticDto
    {
        public IDictionary<DateTime, int> DateCountOfTicketsDictionary { get; set; }

        public IDictionary<Status, int> StatusCountDictionary { get; set; }

        public IDictionary<Priority, int> PriorityCountDictionary { get; set; }

        public IEnumerable<TicketDto> Tickets { get; set; }
    }
}