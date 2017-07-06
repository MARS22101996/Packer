using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerUI.ViewModels.StatisticViewModels
{
    public class DashboardViewModel
    {
        [Required]
        public Guid SelectedTeamId { get; set; }

        public IEnumerable<TeamViewModel> Teams { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
    }
}