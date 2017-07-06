using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerUI.ApiModels
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