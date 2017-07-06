using System;
using System.ComponentModel.DataAnnotations;
using NotificationService.Core.Enums;

namespace NotificationService.WEB.Models
{
    public class TicketApiModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public UserApiModel Assignee { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public Priority Priority { get; set; }
    }
}