using System;

using StatisticService.Core.Enums;

namespace StatisticService.WEB.Models.StatisticApiModels
{
    public class TicketApiModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public UserApiModel Assignee { get; set; }

        public Status Status { get; set; }

        public Priority Priority { get; set; }

        public DateTime CreationDate { get; set; }
    }
}