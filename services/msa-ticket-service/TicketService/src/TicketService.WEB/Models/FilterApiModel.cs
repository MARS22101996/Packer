using System;
using System.Collections.Generic;
using TicketService.Core.Enums;

namespace TicketService.WEB.Models
{
    public class FilterApiModel
    {
        private const int DefaultPageNumber = 1;
        private const int DefaultPageSize = 100;

        public FilterApiModel()
        {
            Page = DefaultPageNumber;
            PageSize = DefaultPageSize;
        }

        public IEnumerable<TicketApiModel> Tickets { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public List<Status> SelectedStatuses { get; set; }

        public List<Priority> SelectedPriorities { get; set; }

        public PageApiModel PageApiModel { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}