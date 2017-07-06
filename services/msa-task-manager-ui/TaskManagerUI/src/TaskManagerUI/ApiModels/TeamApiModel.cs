using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerUI.ApiModels
{
    public class TeamApiModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public UserApiModel Owner { get; set; }
    }
}