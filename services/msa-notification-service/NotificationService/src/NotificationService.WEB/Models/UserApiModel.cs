using System;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.WEB.Models
{
    public class UserApiModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }
    }
}