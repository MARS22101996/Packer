using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaskManagerUI.ViewModels.AccountViewModels;

namespace TaskManagerUI.ViewModels
{
    public class ParticipantsViewModel
    {
        [Required]
        public string SelectedUserEmail { get; set; }

        public TeamViewModel SelectedTeam { get; set; }

        public IEnumerable<UserViewModel> Participants { get; set; }

        public IEnumerable<TeamViewModel> Teams { get; set; }
    }
}