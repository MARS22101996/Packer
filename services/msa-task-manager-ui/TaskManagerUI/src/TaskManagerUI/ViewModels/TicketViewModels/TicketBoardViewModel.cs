using System.Collections.Generic;

namespace TaskManagerUI.ViewModels.TicketViewModels
{
    public class TicketBoardViewModel
    {
        public IEnumerable<TicketViewModel> PlannedTasks { get; set; }

        public IEnumerable<TicketViewModel> ProgressTasks { get; set; }

        public IEnumerable<TicketViewModel> DoneTasks { get; set; }
    }
}