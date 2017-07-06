using System.Collections.Generic;
using TaskManagerUI.ViewModels.CommentViewModels;

namespace TaskManagerUI.ViewModels.TicketViewModels
{
    public class TicketDetailsViewModel
    {
        public IEnumerable<CommentViewModel> Comments { get; set; }

        public TicketViewModel Ticket { get; set; }

        public string CreationDate 
            => Ticket.CreationDate.ToString("MMM dd") + " at " + Ticket.CreationDate.ToString("hh:mm tt");
    }
}