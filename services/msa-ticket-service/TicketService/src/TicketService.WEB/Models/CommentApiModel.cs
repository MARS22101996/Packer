using System;
using System.ComponentModel.DataAnnotations;

namespace TicketService.WEB.Models
{
    public class CommentApiModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime Date { get; set; }
        
        public UserApiModel User { get; set; }
    }
}