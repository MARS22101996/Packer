using System.ComponentModel.DataAnnotations;
using TicketService.Core.Enums;

namespace TicketService.WEB.Models
{
    public class NotificationInfoApiModel
    {
        [Required]
        public NotificationType NotificationType { get; set; }

        public TicketApiModel NewTicket { get; set; }

        public TicketApiModel OldTicket { get; set; }
    }
}