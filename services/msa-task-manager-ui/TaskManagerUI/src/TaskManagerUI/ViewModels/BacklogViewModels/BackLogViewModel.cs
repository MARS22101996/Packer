using System;
using System.Collections.Generic;
using TaskManagerUI.Models.Enums;
using TaskManagerUI.ViewModels.TicketViewModels;

namespace TaskManagerUI.ViewModels.BacklogViewModels
{
    public class BackLogViewModel
    {
        private const int DefaultPageNumber = 1;
        private const int DefaultPageSize = 5;

        public BackLogViewModel()
        {
            Page = DefaultPageNumber;
            PageSize = DefaultPageSize;
        }

        public TeamViewModel SelectedTeam { get; set; }

        public IEnumerable<TeamViewModel> Teams { get; set; }

        public IEnumerable<TicketViewModel> Tickets { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public List<string> SelectedStatuses { get; set; }

        public List<string> SelectedPriorities { get; set; }

        public PageViewModel PageViewModel { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public Status Status { get; set; }

        public Priority Priority { get; set; }

        public bool CheckIfStatusExistsIn
        {
            get
            {
                var isStatusExistIn = SelectedStatuses != null && SelectedStatuses.Contains(Status.ToString());
                
                return isStatusExistIn;
            }
        }

        public bool CheckIfPriorityExistsIn
        {
            get
            {
                var isPriorityExistIn = SelectedPriorities != null && SelectedPriorities.Contains(Priority.ToString());

                return isPriorityExistIn;
            }
        }

        public Guid TeamId { get; set; }
    }
}