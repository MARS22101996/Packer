using System.Collections.Generic;
using TaskManagerUI.ViewModels.TicketViewModels;

namespace TaskManagerUI.ViewModels
{
    public class TeamBoardViewModel
    {
        public TeamViewModel SelectedTeam { get; set; }

        public IEnumerable<TeamViewModel> Teams { get; set; }

        public TicketBoardViewModel SelectedTeamTickets { get; set; }
    }
}