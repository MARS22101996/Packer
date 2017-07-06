using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TaskManagerUI.ApiModels;
using TaskManagerUI.Models.Enums;
using TaskManagerUI.ViewModels.AccountViewModels;

namespace TaskManagerUI.ViewModels.TicketViewModels
{
    public class CreateTicketViewModel
    {
        public CreateTicketViewModel()
        {
            LinkedTickets = new List<TicketApiModel>();
            AllTickets = new List<TicketApiModel>();
        }

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name field is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Text field is required")]
        public string Text { get; set; }

        public UserViewModel Assignee { get; set; }

        public IEnumerable<UserApiModel> Assignees { get; set; }

        public Guid TeamId { get; set; }

        public Status Status { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<string> AllTags { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Priority Priority { get; set; }

        public DateTime CreationDate { get; set; }

        public IEnumerable<TicketApiModel> LinkedTickets { get; set; }

        public IEnumerable<TicketApiModel> AllTickets { get; set; }
    }
}
