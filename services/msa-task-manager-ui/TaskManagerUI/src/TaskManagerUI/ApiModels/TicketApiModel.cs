using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TaskManagerUI.Models.Enums;

namespace TaskManagerUI.ApiModels
{
    public class TicketApiModel
    {
        public TicketApiModel()
        {
            LinkedTicketIds = new List<Guid>();
            Tags = new List<string>();
        }

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name field is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Text field is required")]
        public string Text { get; set; }

        public Guid TeamId { get; set; }

        public UserApiModel Assignee { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public Status Status { get; set; }

        public Priority Priority { get; set; }

        public DateTime CreationDate { get; set; }

        public int CommentCount { get; set; }

        public IEnumerable<Guid> LinkedTicketIds { get; set; }
    }
}