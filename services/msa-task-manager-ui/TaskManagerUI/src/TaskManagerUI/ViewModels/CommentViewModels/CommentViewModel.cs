using System;
using System.ComponentModel.DataAnnotations;
using TaskManagerUI.ViewModels.AccountViewModels;

namespace TaskManagerUI.ViewModels.CommentViewModels
{
    public class CommentViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime Date { get; set; }

        public UserViewModel User { get; set; }

        public Guid TeamId { get; set; }

        public Guid TicketId { get; set; }

        public string DateString
            => Date.ToString("MMM dd") + " at " + Date.ToString("hh:mm tt");
    }
}