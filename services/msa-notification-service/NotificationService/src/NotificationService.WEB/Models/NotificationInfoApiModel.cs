using System.ComponentModel.DataAnnotations;
using NotificationService.Core.Enums;

namespace NotificationService.WEB.Models
{
    public class NotificationInfoApiModel
    {
        [Required]
        public NotificationType NotificationType { get; set; }

        public TicketApiModel NewTicket { get; set; }

        [Required]
        public TicketApiModel OldTicket { get; set; }
    }
}