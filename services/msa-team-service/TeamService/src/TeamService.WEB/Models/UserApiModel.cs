using System;
using System.ComponentModel.DataAnnotations;

namespace TeamService.WEB.Models
{
    public class UserApiModel
    {
        [Required]
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
    }
}