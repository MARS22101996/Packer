using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TaskManagerUI.Models.Enums;
using TaskManagerUI.ViewModels.AccountViewModels;
using TaskManagerUI.ViewModels.CommentViewModels;

namespace TaskManagerUI.ViewModels.TicketViewModels
{
    public class TicketViewModel
    {
        public TicketViewModel()
        {
            LinkedTickets = new List<TicketViewModel>();
            UnlinkedTickets = new List<TicketViewModel>();
            Watchers = new List<UserViewModel>();
            Comments = new List<CommentViewModel>();
            LinkedTicketIds = new LinkedList<Guid>();
        }

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name field is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Text field is required")]
        public string Text { get; set; }

        public UserViewModel Assignee { get; set; }

        public IEnumerable<UserViewModel> Watchers { get; set; }

        public Guid TeamId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Status Status { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string AllTags { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Priority Priority { get; set; }

        public DateTime CreationDate { get; set; }

        public string FormattedCreationDate => CreationDate.ToString("MMM dd") + " at " + CreationDate.ToString("hh:mm tt");

        public int CommentCount { get; set; }

        public IEnumerable<TicketViewModel> LinkedTickets { get; set; }

        public IEnumerable<Guid> LinkedTicketIds { get; set; }

        public int LinkedTicketCount => LinkedTicketIds.Count();

        public IEnumerable<TicketViewModel> UnlinkedTickets { get; set; }

        public IEnumerable<CommentViewModel> Comments { get; set; }
    }
}