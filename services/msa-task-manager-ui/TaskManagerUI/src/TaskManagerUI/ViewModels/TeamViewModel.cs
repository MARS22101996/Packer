using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaskManagerUI.ApiModels;
using TaskManagerUI.ViewModels.AccountViewModels;
using TaskManagerUI.ViewModels.TicketViewModels;

namespace TaskManagerUI.ViewModels
{
    public class TeamViewModel
    {
        public TeamViewModel()
        {
            Participants = new List<UserViewModel>();
            Tickets = new List<TicketViewModel>();
        }

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Field is required")]
        public string Name { get; set; }

        public UserApiModel Owner { get; set; }

        public IEnumerable<UserViewModel> Participants { get; set; }

        public IEnumerable<TicketViewModel> Tickets { get; set; }
    }
}
