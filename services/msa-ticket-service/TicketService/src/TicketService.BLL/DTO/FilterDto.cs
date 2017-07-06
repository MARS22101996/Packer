using System;
using System.Collections.Generic;
using TicketService.Core.Enums;

namespace TicketService.BLL.DTO
{
    public class FilterDto
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public IEnumerable<Status> SelectedStatuses { get; set; }

        public IEnumerable<Priority> SelectedPriorities { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}