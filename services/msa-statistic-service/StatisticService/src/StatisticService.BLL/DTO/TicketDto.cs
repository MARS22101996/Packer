using System;
using System.Collections.Generic;
using StatisticService.Core.Enums;

namespace StatisticService.BLL.DTO
{
    public class TicketDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public UserDto Assignee { get; set; }

        public Status Status { get; set; }

        public Priority Priority { get; set; }

        public DateTime CreationDate { get; set; }
    }
}