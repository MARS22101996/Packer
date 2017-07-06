using System;

namespace TicketService.WEB.Models
{
    public class UserApiModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
    }
}