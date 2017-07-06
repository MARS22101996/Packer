using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicketService.Core.Enums;

namespace TicketService.WEB.Models
{
    public class TicketApiModel
    {
        public TicketApiModel()
        {
            LinkedTicketIds = new List<Guid>();
            Tags = new List<string>();
        }

        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Text { get; set; }

        public UserApiModel Assignee { get; set; }

        public Status Status { get; set; }

        public Priority Priority { get; set; }

        public DateTime CreationDate { get; set; }

        public IEnumerable<Guid> LinkedTicketIds { get; set; }

        public int CommentCount { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}